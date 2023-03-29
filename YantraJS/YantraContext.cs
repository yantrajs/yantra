using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Threading;
using NuGet.Versioning;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Utils;
using YantraJS.Network;

namespace YantraJS
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
            name = name.Split(',')[0].Trim();
            if (assemblyCache.TryGetValue(name, out v))
            {
                if (System.IO.File.Exists(v))
                    return AppDomain.CurrentDomain.Load(System.IO.File.ReadAllBytes(v));
            }
            return null;
        }

        readonly string folder;
        public YantraContext(string folder, SynchronizationContext ctx = null): base(ctx)
        {
            this.folder = folder;
            // reverse priority, select csx before js.
            this.extensions = new string[] { ".csx", ".js"  };

            this[KeyStrings.console] = (typeof(YantraConsole)).Marshal();

            this.InstallNetworkServices();
        }

        private async Task<JSModuleDelegate> LoadDelegate(string assemblyPath)
        {
            var returnType = typeof(Task<JSModuleDelegate>);
            var data = await ReadAllBytesAsync(assemblyPath);
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
                    Task Module(JSModule module)
                    {
                        module.Exports = ClrType.From(type);
                        return Task.CompletedTask;
                    }
                    return Module;
                }
                exportedTypes.Add((export.Name, type));
            }
            var et = exportedTypes.ToArray();
            Task CaptureTypes(JSModule module) {
                var exports = module.Exports;
                foreach(var (name, type) in et)
                {
                    exports[name] = ClrType.From(type);
                }
                return Task.CompletedTask;
            }
            return CaptureTypes;

        }

        private async Task<byte[]> ReadAllBytesAsync(string assemblyPath)
        {
            using(var fs = System.IO.File.OpenRead(assemblyPath))
            {
                var ms = new MemoryStream();
                await fs.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        private async Task<string> ReadAllTextAsync(string filePath)
        {
            using (var ss = new StreamReader(filePath))
            {
                return await ss.ReadToEndAsync();
            }

        }


        async Task<JSModuleDelegate> LoadDelegate(FileInfo dllFile, FileInfo depsFile)
        {
            if (depsFile.Exists)
            {
                var text = await ReadAllTextAsync(depsFile.FullName);
                var files = System.Text.Json.JsonSerializer.Deserialize<DependentAssemblyName[]>(text);
                foreach(var file in files)
                {
                    // var name = System.IO.Path.GetFileNameWithoutExtension(file);
                    assemblyCache.TryAdd(file.Name, file.Path);
                    assemblyCache.TryAdd(file.FullName, file.Path);
                }
            }
            return await LoadDelegate(dllFile.FullName);
        }

        protected override async Task CompileModuleAsync(JSModule module)
        {

            var filePath = module.filePath;

            if (!filePath.EndsWith(".csx"))
            {
                await base.CompileModuleAsync(module);
                return;
            }


            JSModuleDelegate @delegate = null;
                var originalFile = new FileInfo(filePath);
                var dllFile = new FileInfo(filePath + ".dll");
                var depsFile = new FileInfo(filePath + ".dll.deps");


            if (dllFile.Exists && dllFile.LastWriteTimeUtc >= originalFile.LastWriteTimeUtc)
            {
                @delegate = await LoadDelegate(dllFile, depsFile);
            }
            else
            {

                using var reader = new StreamReader(filePath, Encoding.UTF8);
                var code = await reader.ReadToEndAsync();

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
                    .Select((name) => new DependentAssemblyName
                    {
                        Name = name.name.Name,
                        FullName = name.name.FullName,
                        Path = name.path
                    })
                    .ToArray();
                // emit .deps.json
                System.IO.File.WriteAllText(depsFile.FullName, System.Text.Json.JsonSerializer.Serialize(deps));


                if (!er.Success)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var d in er.Diagnostics)
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

            await @delegate(module);
        }

        

        static string RemoveReference(string code)
        {
            StringBuilder sb = new StringBuilder();
            StringReader sr = new StringReader(code);
            string line;
            while ((line = sr.ReadLine() ) != null)
            {
                var l = line.TrimStart();
                if(l.StartsWith("#r \"nuget: "))
                {
                    var name = ParseName(l);
                    if (name == "YantraJS.Core")
                        continue;
                }
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        private static string ParseName(string text)
        {
            int index = text.IndexOf(':');
            if (index == -1)
                return text;
            text = text.Substring(index + 1);
            index = text.IndexOf(',');
            if (index == -1)
                return text;
            return text.Substring(0, index).Trim();
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
