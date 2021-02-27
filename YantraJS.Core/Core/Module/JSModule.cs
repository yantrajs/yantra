using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace YantraJS.Core
{
    /// <summary>
    /// Create and load a module
    /// </summary>
    public class JSModule: JSObject
    {

        internal readonly string filePath;
        internal readonly string dirPath;
        private JSFunctionDelegate factory;

        public JSModule(JSObject exports, string name)
        {
            this.filePath = name;
            this.dirPath = "./";
            this.exports = exports;
        }

        protected JSModule(string name)
        {
            this.filePath = name;
            this.dirPath = "./";
        }

        internal JSModule(
            JSModuleContext context,
            string filePath,
            string code,
            bool main = false) : base(context.ModulePrototype)
        {
            // this.ownProperties = new PropertySequence();

            this.filePath = filePath;
            this.dirPath = System.IO.Path.GetDirectoryName(filePath);
            this.exports = new JSObject();

            Console.WriteLine($"{DateTime.Now} - Compiling module {filePath}");

            // if this is a json file... then pad with module.exports = 

            if (this.filePath.EndsWith(".json"))
            {
                code = $"module.exports = {code};";
            }

            this.factory = context.Compile(code, filePath, new List<string> {
                "exports",
                "require",
                "module",
                "__filename",
                "__dirname"
            });
            Console.WriteLine($"{DateTime.Now} - Compiling module {filePath} finished ..");
            var require = Require = new JSFunction((in Arguments a1) => { 
                var r = context.LoadModule(this, a1);
                return r;
            });
            require["main"] = main ? JSBoolean.True : JSBoolean.False;
            var resolve = new JSFunction((in Arguments ar) => {
                var f = ar.Get1();
                if (!f.IsString)
                    throw context.NewTypeError("First parameter is not string");
                return new JSString(context.Resolve(dirPath, f.ToString()));
            });
            require[KeyStrings.resolve] = resolve;

        }

        [Prototype("id")]
        public JSValue Id => new JSString(filePath);

        JSValue exports;
        [Prototype("exports")]
        public JSValue Exports {
            get {
                var f = this.factory;
                if (f != null)
                {
                    this.factory = null;
                    f(new Arguments(this, new JSValue[] {
                        exports, Require, this, new JSString(filePath), new JSString(dirPath)
                    }));
                }
                return exports;
            }
            set
            {
                if (value == null || value.IsNullOrUndefined)
                {
                    throw JSContext.Current.NewTypeError("Exports cannot be set to null or undefined");
                }
                exports = value;
            }
        }

        [Prototype("require")]
        public JSValue Require { get; set; }

    }
}
