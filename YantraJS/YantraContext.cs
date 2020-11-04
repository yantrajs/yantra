using Esprima.Ast;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Threading;
using Newtonsoft.Json;
using NuGet.Versioning;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Core.Clr;
using Yantra.Utils;

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

        private static ConcurrentDictionary<string, string> assemblyCache = new ConcurrentDictionary<string, string>();

        static YantraContext()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name;
            if(assemblyCache.TryGetValue(name, out var v))
            {
                if (System.IO.File.Exists(v))
                    return AppDomain.CurrentDomain.Load(System.IO.File.ReadAllBytes(v));
            }
            name = name.Split(",")[0].Trim();
            if (assemblyCache.TryGetValue(name, out v))
            {
                if (System.IO.File.Exists(v))
                    return AppDomain.CurrentDomain.Load(System.IO.File.ReadAllBytes(v));
            }
            return null;
        }

        readonly string folder;
        public YantraContext(string folder)
        {
            this.folder = folder;
            // reverse priority, select csx before js.
            this.extensions = new string[] { ".csx", ".js"  };

            this["console"] = (typeof(YantraConsole)).Marshal();
        }

        private async Task<JSModuleDelegate> LoadDelegate(string assemblyPath)
        {
            var returnType = typeof(Task<JSModuleDelegate>);
            var data = await System.IO.File.ReadAllBytesAsync(assemblyPath);
            var a = AppDomain.CurrentDomain.Load(data);
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
                foreach(var (name, type) in et)
                {
                    exports[name] = ClrType.From(type);
                }
            }
            return CaptureTypes;

        }

        async Task<JSModuleDelegate> LoadDelegate(FileInfo dllFile, FileInfo depsFile)
        {
            if (depsFile.Exists)
            {
                var text = await System.IO.File.ReadAllTextAsync(depsFile.FullName);
                var files = JsonConvert.DeserializeObject<DependentAssemblyName[]>(text);
                foreach(var file in files)
                {
                    // var name = System.IO.Path.GetFileNameWithoutExtension(file);
                    assemblyCache.TryAdd(file.Name, file.Path);
                    assemblyCache.TryAdd(file.FullName, file.Path);
                }
            }
            return await LoadDelegate(dllFile.FullName);
        }
        protected override JSFunctionDelegate Compile(string code, string filePath, List<string> args)
        {
            if (!filePath.EndsWith(".csx"))
                return base.Compile(code, filePath, args);


            JSModuleDelegate @delegate = null;
            AsyncPump.Run(async () =>
            {
                var originalFile = new FileInfo(filePath);
                var dllFile = new FileInfo(filePath + ".dll");
                var depsFile = new FileInfo(filePath + ".dll.deps");


                if (dllFile.Exists && dllFile.LastWriteTimeUtc >= originalFile.LastWriteTimeUtc)
                {
                    @delegate = await LoadDelegate(dllFile, depsFile);
                } else {

                    var nugetResolver = new NuGetMetadataReferenceResolver(
                        ScriptMetadataResolver.Default.WithBaseDirectory(originalFile.DirectoryName), folder);

                    var options = ScriptOptions.Default
                            .WithFilePath(filePath)
                            .AddReferences(typeof(JSValue).Assembly, typeof(YantraContext).Assembly)
                            .WithMetadataResolver(nugetResolver)
                            .WithOptimizationLevel(OptimizationLevel.Debug);

                    var oldCode = code;
                    // remove yantra code 
                    code = RemoveReference(code);

                    var csCode = await CSharpScript.RunAsync<JSModuleDelegate>(code, options);

                    var r = csCode.ReturnValue;
                    @delegate = r;

                    var c = csCode.Script.GetCompilation();

                    var er = c.Emit(dllFile.FullName);

                    var ers = c.DirectiveReferences;
                    var deps = ers.Select(d => (name: AssemblyName.GetAssemblyName(d.Display), path: d.Display))
                        .Select((name) => new DependentAssemblyName { 
                            Name = name.name.Name,
                            FullName = name.name.FullName,
                            Path = name.path
                        })
                        .ToArray();
                    // emit .deps.json
                    System.IO.File.WriteAllText(depsFile.FullName, JsonConvert.SerializeObject(deps));


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
                        @delegate = await LoadDelegate(dllFile, depsFile);
                    }

                }

            });
            return (in Arguments a) => {
                var alist = a.GetArgs();
                @delegate(alist[0], alist[1], alist[2], alist[3].ToString(), alist[4].ToString());
                return JSUndefined.Value;
            };
        }

        

        static string RemoveReference(string code)
        {
            StringBuilder sb = new StringBuilder();
            StringReader sr = new StringReader(code);
            string line;
            while ((line = sr.ReadLine() ) != null)
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

    public class DependentAssemblyName
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Path { get; set; }
    }
}
