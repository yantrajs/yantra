using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebAtoms.CoreJS.Core
{
    /// <summary>
    /// Enables Modules, both CommonJS and ES Modules
    /// </summary>
    public class JSModuleContext : JSContext
    {
        public new static JSModuleContext Current => JSContext.Current as JSModuleContext;

        internal readonly JSObject ModulePrototype;

        public JSModuleContext()
        {
            ModulePrototype = this.Create<JSModule>(KeyStrings.Module, null, false).prototype;
        }

        /// <summary>
        /// Modules are isolated by Context and are identified by Id.
        /// 
        /// Specially in server environment with multiple context, module names
        /// are identified by unique id present in ModuleName.
        /// </summary>
        private ConcurrentUInt32Trie<JSModule> moduleCache
            = new ConcurrentUInt32Trie<JSModule>();
        
        private string[] paths;



        internal string Resolve(string dirPath, string relativePath)
        {
            string Combine(string cFolder, string cR, string ext = ".js")
            {
                string cP = Path.Combine(cFolder, cR) + ext;
                if (File.Exists(cP))
                    return cP;
                return null;
            }

            if (relativePath.StartsWith("."))
            {
                return Combine(dirPath, relativePath);
            }

            foreach(var folder in paths)
            {
                string path = Combine(folder, relativePath);
                if (path != null)
                    return path;
                path = Combine(folder, relativePath + "/index");
                if (path != null)
                    return path;
            }
            return null;
        }

        void UpdatePaths(string[] paths = null)
        {
            this.paths = paths ?? new string[] { 
                this.CurrentPath,
                this.CurrentPath + "/node_modules",
                // system npm paths...
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/yantra/node_modules"
            };
        }

        public async static Task<JSValue> RunExportsAsync(
            string folder, 
            string relativeFile,
            string exportedFunctionName,
            Arguments a,
            string[] paths = null
            )
        {
            using (var m = new JSModuleContext())
            {
                m.CurrentPath = folder;
                m.UpdatePaths(paths);
                var filePath = m.Resolve(folder, 
                    relativeFile.StartsWith(".") ? 
                    relativeFile : ( "./" + relativeFile));
                if (filePath == null)
                    throw new FileNotFoundException($"{filePath} not found");
                string text;
                using(var fs = File.OpenText(filePath))
                {
                    text = await fs.ReadToEndAsync();
                }

                var main = m.Main = new JSModule(m, filePath, text, true);

                var exported = main.Exports[exportedFunctionName];
                if (exported.IsUndefined)
                    throw new KeyNotFoundException($"{exportedFunctionName} not found on the module");
                var rv = exported.InvokeFunction(a);
                if (rv is JSPromise promise)
                {
                    return await promise.Task;
                }
                if (m.waitTask != null)
                    await m.waitTask;
                return rv;
            }
            
        }

        public string CurrentPath { get; set; }

        public JSModule Main { get; set;}

        internal JSValue LoadModule(JSModule callee, in Arguments a)
        {
            var name = a.Get1();
            if (!name.IsString)
                throw NewTypeError("require method's parameter must be a string");
            var relativePath = name.ToString();

            // resolve full name..
            var fullPath = Resolve(callee.dirPath, relativePath);
            if (fullPath == null)
                throw new FileNotFoundException($"{relativePath} module not found");
            ModuleName moduleName = fullPath;
            var code = System.IO.File.ReadAllText(fullPath);
            JSModule module = moduleCache.GetOrCreate(moduleName.Id, () => new JSModule(this, fullPath, code));
            return module.Exports;
        }


    }
}
