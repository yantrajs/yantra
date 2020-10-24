using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core.Clr;
using WebAtoms.CoreJS.Core.Storage;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultExportAttribute: ExportAttribute
    {
        public DefaultExportAttribute(): base(KeyStrings.@default)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExportAttribute : Attribute
    {
        public KeyString Name { get; }

        protected ExportAttribute(KeyString n) {
            Name = n;
        }

        /// <summary>
        /// Exports given Type as class
        /// </summary>
        /// <param name="name">Asterix '*' if null</param>
        public ExportAttribute(string name = null)
        {

        }
    }

    public delegate void JSModuleDelegate(
    JSValue exports,
    JSValue require,
    JSValue module,
    string __filename,
    string __dirname
    );

    internal class ModuleCache: ConcurrentSharedStringTrie<JSModule>
    {
        internal static Key module = "module";
        internal static Key clr = "clr";
    }

    /// <summary>
    /// Enables Modules, both CommonJS and ES Modules
    /// </summary>
    public class JSModuleContext : JSContext
    {
        internal readonly JSObject ModulePrototype;
        internal readonly JSFunction Module;

        public JSModuleContext()
        {
            this.CreateSharedObject(KeyStrings.assert, typeof(JSAssert), true);

            Module = this.Create<JSModule>(KeyStrings.Module, null, false);
            ModulePrototype = Module.prototype;

            moduleCache[ModuleCache.module] = new JSModule(Module, "module");

            moduleCache[ModuleCache.clr] = new JSModule(ClrModule.Default, "clr");
        }

        /// <summary>
        /// Modules are isolated by Context and are identified by Id.
        /// 
        /// Specially in server environment with multiple context, module names
        /// are identified by unique id present in ModuleName.
        /// </summary>
        private ModuleCache moduleCache
            = new ModuleCache();
        
        private string[] paths;

        protected string[] extensions = new string[] { ".js" };

        internal string Resolve(string dirPath, string relativePath)
        {
            string Combine(string cFolder, string cR, bool checkExtensions = true)
            {
                string cP = Path.Combine(cFolder, cR);
                if (File.Exists(cP))
                    return cP;
                if (!checkExtensions)
                    return null;
                foreach (var e in extensions)
                {
                    var cpE = Combine(cFolder, cR + e, false);
                    if (cpE != null)
                        return cpE;
                }
                return null;
            }

            if (relativePath.StartsWith("."))
            {
                return Combine(dirPath, relativePath);
            }

            foreach(var folder in paths)
            {
                var path = Combine(folder, relativePath);
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
            if (paths != null)
            {
                var np = new string[paths.Length + 2];
                np[0] = this.CurrentPath;
                np[1] = this.CurrentPath + "/node_modules";
                Array.Copy(paths, 0, np, 2, paths.Length);
                paths = np;
            }
            this.paths = paths ?? new string[] { 
                this.CurrentPath,
                this.CurrentPath + "/node_modules",
                // system npm paths...
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/yantra/node_modules"
            };
        }


        public async Task<JSValue> RunAsync(string folder, string relativeFile, string[] paths = null) {
            CurrentPath = folder;
            UpdatePaths(paths);
            var filePath = Resolve(folder,
                relativeFile.StartsWith(".") ?
                relativeFile : ("./" + relativeFile));
            if (filePath == null)
                throw new FileNotFoundException($"{filePath} not found");
            string text;
            using (var fs = File.OpenText(filePath))
            {
                text = await fs.ReadToEndAsync();
            }
            Main = new JSModule(this, filePath, text, true);
            var r = Main.Exports;
            if (waitTask != null)
                await waitTask;
            if (r is JSPromise promise)
            {
                return await promise.Task;
            }
            if (waitTask != null)
                await waitTask;
            return r;

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

            // fetch system modules 
            if (moduleCache.TryGetValue(relativePath, out var m))
                return m.Exports;

            // resolve full name..
            var fullPath = Resolve(callee.dirPath, relativePath);
            if (fullPath == null)
                throw new FileNotFoundException($"{relativePath} module not found");
            var code = System.IO.File.ReadAllText(fullPath);
            JSModule module = moduleCache.GetOrCreate(fullPath, () => new JSModule(this, fullPath, code));
            var exports = module.Exports;
            return exports;
        }
        internal protected virtual JSFunctionDelegate Compile(string code, string filePath, List<string> args)
        {
            return CoreScript.Compile(code, filePath, args);
        }


    }
}
