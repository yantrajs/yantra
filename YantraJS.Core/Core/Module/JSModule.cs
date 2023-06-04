using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yantra.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Core
{
    /// <summary>
    /// Create and load a module
    /// </summary>

    [JSBaseClass("Object")]
    [JSFunctionGenerator("Module", Register = false)]
    public partial class JSModule: JSObject
    {
        private readonly JSModuleContext context;
        public readonly string filePath;
        internal readonly string dirPath;

        [JSPrototypeMethod][JSExport("code")]
        public string Code { get; set; }

        public JSModule(in Arguments a)
        {
            throw new NotSupportedException();
        }

        public JSModule(JSModuleContext context, JSObject exports, string name, bool isMain = false)
            : this(context.ModulePrototype)
        {
            this.context = context;
            this.filePath = name;
            this.dirPath = "./";
            this.exports = exports;
            this.IsMain = isMain;
        }

        internal JSModule(JSModuleContext context, string name, string code = null)
            : this(context.ModulePrototype)
        {
            this.context = context;
            this.filePath = name;
            this.dirPath = System.IO.Path.GetDirectoryName(dirPath);
            Code = code;
        }

        [JSPrototypeMethod][JSExport("id")]
        public JSValue Id => new JSString(filePath);

        JSValue exports;
        private bool IsMain;

        [JSPrototypeMethod][JSExport("exports")]
        public JSValue Exports {
            get {
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

        [JSPrototypeMethod][JSExport("require")]
        public JSValue Require { get; set; }

        [JSPrototypeMethod][JSExport("import")]
        public JSValue Import { get; set; }

        public Task<JSValue> ImportAsync(string name)
        {
            var result = Import.InvokeFunction(new Arguments(JSUndefined.Value, new JSString(name)));
            return (result as JSPromise).Task;
        }

        [JSPrototypeMethod][JSExport("compile")]
        public JSValue Compile { get; set; }
        
        internal async Task InitAsync()
        {
            if (exports != null)
            {
                return;
            }
            exports = new JSObject();
            var result = this.Compile.InvokeFunction(new Arguments(this));
            if (result is JSPromise promise)
                await promise.Task;
        }
    }
}
