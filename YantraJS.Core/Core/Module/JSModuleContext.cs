using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.Storage;
using YantraJS.Utils;

namespace YantraJS.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultExportAttribute: ExportAttribute
    {
        public DefaultExportAttribute(): base("default")
        {

        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExportAttribute : Attribute
    {
        public string Name { get; }

        /// <summary>
        /// Exports given Type as class
        /// </summary>
        /// <param name="name">Asterix '*' if null</param>
        public ExportAttribute(string name = null)
        {
            this.Name = name;
        }
    }

    public delegate void JSModuleDelegate(
    JSValue exports,
    JSValue require,
    JSValue module,
    string __filename,
    string __dirname
    );

    //internal class ModuleCache: ConcurrentSharedStringTrie<JSModule>
    //{
    //    internal static Key module = "module";
    //    internal static Key clr = "clr";
    //}

    public struct ModuleCache
    {
        private static ConcurrentNameMap nameCache;
        private ConcurrentUInt32Map<JSModule> modules;
        

        static ModuleCache()
        {
            nameCache = new ConcurrentNameMap();
            module = nameCache.Get("module");
            clr = nameCache.Get("clr");
        }

        public static (uint Key, StringSpan Name) module;
        public static (uint Key, StringSpan Name) clr;

        public static ModuleCache Create()
        {
            return new ModuleCache(true);
        }
        public bool TryGetValue(in StringSpan key, out JSModule obj)
        {
            if(nameCache.TryGetValue(key, out var i))
            {
                if (modules.TryGetValue(i.Key, out obj))
                    return true;
            }
            obj = null;
            return false;
        }
        public JSModule GetOrCreate(in StringSpan key, Func<JSModule> factory)
        {
            var k = nameCache.Get(key);
            return modules.GetOrCreate(k.Key, factory);
        }

        public ModuleCache(bool v)
        {
            modules = ConcurrentUInt32Map<JSModule>.Create();
        }

        public JSModule this[in (uint Key, StringSpan name) key]
        {
            get {
                if (modules.TryGetValue(key.Key, out var m))
                    return m;
                return null;
            }
            set {
                modules[key.Key] = value;
            }
        }

        public IEnumerable<JSModule> All => modules.All;
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

            this[KeyStrings.globalThis] = this;
            this[KeyStrings.global] = this;
        }

        /// <summary>
        /// Modules are isolated by Context and are identified by Id.
        /// 
        /// Specially in server environment with multiple context, module names
        /// are identified by unique id present in ModuleName.
        /// </summary>
        readonly ModuleCache moduleCache
            = ModuleCache.Create();

        [Browsable(false)]
        public IEnumerable<JSModule> All
        {
            get
            {
                return moduleCache.All;
            }
        }

        private string[] paths;

        protected string[] extensions = new string[] { ".js" };

        internal string Resolve(string dirPath, string relativePath)
        {
            bool Exists(string folder, string file, out string path)
            {
                string fullName = Path.Combine(folder, file);
                if (!file.StartsWith("."))
                {
                    if (System.IO.Directory.Exists(fullName))
                    {
                        var pkgJson = fullName + "/package.json";
                        if (System.IO.File.Exists(pkgJson))
                        {
                            var json = System.IO.File.ReadAllText(pkgJson);
                            var pkg = JObject.Parse(json);
                            if (pkg.TryGetValue("main", out var token))
                            {
                                var v = token.Value<string>();
                                path = Path.Combine(fullName, v);
                                return true;
                            }
                        }
                    }
                }
                if(System.IO.File.Exists(fullName))
                {
                    path = fullName;
                    return true;
                }
                path = null;
                return false;
            }
            foreach (var ext in extensions)
            {
                if (relativePath.StartsWith("."))
                {
                    if (Exists(dirPath, relativePath, out var path))
                        return path;
                    if (Exists(dirPath, relativePath + ext, out path))
                        return path;
                    continue;
                }

                foreach (var folder in paths)
                {
                    if (Exists(folder, relativePath, out var path))
                        return path;
                    if (Exists(folder, relativePath + ext, out path))
                        return path;
                    if (Exists(folder, relativePath + "/index" + ext, out path))
                        return path;

                    // check if package.json exists...

                }
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
                throw new FileNotFoundException($"{relativeFile} not found");
            string text;
            using (var fs = File.OpenText(filePath))
            {
                text = await fs.ReadToEndAsync();
            }
            Main = new JSModule(this, filePath, text, true);
            var r = Main.Exports;
            var w = WaitTask;
            if (w != null)
                await w;
            if (r is JSPromise promise)
            {
                return await promise.Task;
            }
            w = WaitTask;
            if (w != null)
                await w;
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
                if (m.WaitTask != null)
                    await m.WaitTask;
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
            //if (moduleCache.TryGetValue(relativePath, out var m))
            //{
            //    return m.Exports;
            //}

            // resolve full name..
            var fullPath = Resolve(callee.dirPath, relativePath);
            if (fullPath == null)
                throw NewTypeError($"{relativePath} module not found");
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
