using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YantraJS.Core
{
    /// <summary>
    /// Create and load a module
    /// </summary>
    public class JSModule: JSObject
    {
        private readonly JSModuleContext context;
        internal readonly string filePath;
        internal readonly string dirPath;

        public JSModule(JSModuleContext context, JSObject exports, string name, bool isMain = false)
            : base(context.ModulePrototype)
        {
            this.context = context;
            this.filePath = name;
            this.dirPath = "./";
            this.exports = exports;
            this.IsMain = isMain;
        }

        protected JSModule(JSModuleContext context, string name, string dirPath = "./")
            : base(context.ModulePrototype)
        {
            this.context = context;
            this.filePath = name;
            this.dirPath = dirPath;
            this.exports = new JSObject();
        }

        internal static async Task<JSModule> CreateAsync(
            JSModuleContext context,
            string filePath,
            string code,
            bool main = false)
        {
            // this.ownProperties = new PropertySequence();
            var @this = new JSModule(context, filePath, System.IO.Path.GetDirectoryName(filePath));

            Console.WriteLine($"{DateTime.Now} - Compiling module {filePath}");

            // if this is a json file... then pad with module.exports = 

            if (filePath.EndsWith(".json"))
            {
                code = $"module.exports = {code};";
            } else
            {
                code = @$"(async function({{module, import, exports, require, filePath: __filename, dirPath: __dirname}}) {{ {code} }})";
            }

            @this[KeyStrings.module] = @this;

            @this.Import = new JSFunction((in Arguments a) => {
                var name = a[0];
                if (!name.IsString)
                    throw context.NewTypeError("require method's parameter must be a string");
                var result = context.LoadModuleAsync(@this, name.StringValue);
                return Clr.ClrProxy.Marshal(result);
            });

            @this.Require = new JSFunction((in Arguments a) => {
                var name = a[0];
                if (!name.IsString)
                    throw context.NewTypeError("require method's parameter must be a string");
                var result = context.LoadModuleAsync(@this, name.StringValue);
                return AsyncPump.Run(() => result);
            });

            var factory = context.FastEval(code, filePath);

            var result = factory.InvokeFunction(new Arguments(@this, @this)) as JSPromise;
            if (result != null)
            {
                await result.Task;
            }

            //this.factory = context.Compile(code, filePath, new List<string> {
            //    "exports",
            //    "require",
            //    "module",
            //    "__filename",
            //    "__dirname"
            //});
            //Console.WriteLine($"{DateTime.Now} - Compiling module {filePath} finished ..");
            //var require = Require = new JSFunction((in Arguments a1) => { 
            //    var r = context.LoadModule(this, a1);
            //    return r;
            //});
            //require["main"] = main ? JSBoolean.True : JSBoolean.False;
            //var resolve = new JSFunction((in Arguments ar) => {
            //    var f = ar.Get1();
            //    if (!f.IsString)
            //        throw context.NewTypeError("First parameter is not string");
            //    return new JSString(context.Resolve(dirPath, f.ToString()));
            //});
            //require[KeyStrings.resolve] = resolve;
            return @this;
        }

        [Prototype("id")]
        public JSValue Id => new JSString(filePath);

        JSValue exports;
        private bool IsMain;

        [Prototype("exports")]
        public JSValue Exports {
            get {
                //var f = this.factory;
                //if (f != null)
                //{
                //    this.factory = null;
                //    f(new Arguments(this, new JSValue[] {
                //        exports, Require, this, new JSString(filePath), new JSString(dirPath)
                //    }));
                //}
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

        [Prototype("import")]
        public JSValue Import { get; set; }

        public Task<JSValue> ImportAsync(string name)
        {
            var path = context.Resolve(this.dirPath, name);
            return context.LoadModuleAsync(this, path);
        }

    }
}
