using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Core.Clr;

namespace Yantra
{

    public delegate void ModuleDelegate(
        JSValue exports,
        JSValue require,
        JSValue module,
        string __filename,
        string __dirname
        );

    public class YantraContext: JSModuleContext
    {

        public YantraContext()
        {
            // reverse priority, select csx before js.
            this.extensions = new string[] { ".csx", ".js"  };
        }

        private async Task<JSModuleDelegate> LoadDelegate(string assemblyPath)
        {
            var returnType = typeof(Task<JSModuleDelegate>);
            var a = Assembly.LoadFile(assemblyPath);
            var methods = a.GetTypes().SelectMany(x => x.GetMethods());
            var p = methods.FirstOrDefault(x => x.IsStatic && x.ReturnType == returnType);
            if (p != null)
            {
                var sa = new object[2];
                var r = p.Invoke(null, new object[] { sa });
                var rr = await (Task<JSModuleDelegate>)r;
                if (rr != null)
                    return rr;
            }

            var exportedTypes = new List<(string name, Type type)>();
            foreach(var type in a.GetTypes())
            {
                var export = type.GetCustomAttribute<ExportAttribute>();
                if (export == null)
                    continue;
                if (export.Name == null) {
                    void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname)
                    {
                        module["exports"] = ClrType.From(type);
                    }
                    return Module;
                }
                exportedTypes.Add((export.Name, type));
            }
            var et = exportedTypes.ToArray();
            void CaptureTypes(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) { 
                foreach(var e in et)
                {
                    exports[e.name] = ClrType.From(e.type);
                }
            }
            return CaptureTypes;

        }

        protected override JSFunctionDelegate Compile(string code, string filePath, List<string> args)
        {
            if (filePath.EndsWith(".csx"))
            {

                JSModuleDelegate @delegate = null;
                AsyncPump.Run(async () =>
                {
                    var originalFile = new FileInfo(filePath);
                    var dllFile = new FileInfo(filePath + ".dll");
                    if (dllFile.Exists && dllFile.LastWriteTimeUtc >= originalFile.LastWriteTimeUtc)
                    {
                        @delegate = await LoadDelegate(dllFile.FullName);
                    } else { 
                        var options = ScriptOptions.Default
                                .WithFilePath(filePath)
                                .AddReferences(typeof(JSValue).Assembly, typeof(YantraContext).Assembly)
                                .WithMetadataResolver(new NuGetMetadataReferenceResolver(ScriptMetadataResolver.Default.WithBaseDirectory(originalFile.DirectoryName)))
                                .WithOptimizationLevel(OptimizationLevel.Debug);

                        var oldCode = code;
                        // remove yantra code 
                        code = RemoveReference(code);

                        var csCode = await CSharpScript.RunAsync<JSModuleDelegate>(code, options);

                        var r = csCode.ReturnValue;
                        @delegate = r;
                        var er = csCode.Script.GetCompilation().Emit(dllFile.FullName);
                        if(!er.Success)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach(var d in er.Diagnostics)
                            {
                                sb.AppendLine(d.ToString());
                            }
                            throw new Exception(sb.ToString());
                        }
                        if (@delegate == null)
                        {
                            @delegate = await LoadDelegate(dllFile.FullName);
                        }

                    }
                });
                return (in Arguments a) => {
                    var alist = a.GetArgs();
                    @delegate(alist[0], alist[1], alist[2], alist[3].ToString(), alist[4].ToString());
                    return JSUndefined.Value;
                };
            }
            return base.Compile(code, filePath, args);
        }
        static string RemoveReference(string code)
        {
            StringBuilder sb = new StringBuilder();
            StringReader sr = new StringReader(code);
            string line = null;
            while((line = sr.ReadLine() ) != null)
            {
                var l = line.TrimStart();
                if(l.StartsWith("nuget:"))
                {
                    l = l.Substring(6).Trim();
                    if (l.StartsWith("YantraJS.Core,"))
                        continue;
                }
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

    }

    public class ModuleGlobals
    {
        public JSValue exports;
        public JSValue require;
        public JSValue module;
        public string __filename;
        public string __dirname;
    }
}
