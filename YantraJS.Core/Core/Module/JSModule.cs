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
        private JSContext context;

        public JSModule(JSObject exports, string name)
        {
            context = JSContext.Current;
            this.filePath = name;
            this.dirPath = "./";
            this.exports = exports;
        }

        protected JSModule(string name)
        {
            context = JSContext.Current;
            this.filePath = name;
            this.dirPath = "./";
        }

        internal JSModule(
            JSModuleContext context, 
            string filePath, 
            string code,
            bool main = false): base(context.ModulePrototype)
        {
            this.context = context;
            // this.ownProperties = new PropertySequence();

            this.filePath = filePath;
            this.dirPath = System.IO.Path.GetDirectoryName(filePath);

            Console.WriteLine($"Compiling module {filePath}");
            this.factory = context.Compile(code, filePath, new List<string> {
                "exports",
                "require",
                "module",
                "__filename",
                "__dirname"
            });
            Console.WriteLine($"Compiling module {filePath} finished ..");
            var exports = Exports = new JSObject();
            var require = Require = new JSFunction((in Arguments a1) => context.LoadModule(this, a1));
            require[context, "main"] = main ? JSBoolean.True : JSBoolean.False;
            var resolve = new JSFunction((in Arguments ar) => {
                var f = ar.Get1();
                if (!f.IsString)
                    throw context.NewTypeError("First parameter is not string");
                return new JSString(context.Resolve(dirPath, f.ToString()));
            });
            require[context, KeyStrings.resolve] = resolve;

        }

        [Prototype("id")]
        public JSValue Id => new JSString(filePath);

        JSValue exports;
        [Prototype("exports")]
        public JSValue Exports {
            get {
                var factory = this.factory;
                this.factory = null;
                factory?.Invoke(new Arguments(context, this, new JSValue[] {
                    exports, Require, this, new JSString(filePath), new JSString(dirPath)
                }));
                return exports;
            }
            set
            {
                if (value == null || value.IsNullOrUndefined)
                {
                    throw context.NewTypeError("Exports cannot be set to null or undefined");
                }
                exports = value;
            }
        }

        [Prototype("require")]
        public JSValue Require { get; set; }

    }
}
