using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public readonly string filePath;
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

        internal JSModule(JSModuleContext context, string name)
            : base(context.ModulePrototype)
        {
            this.context = context;
            this.filePath = name;
            this.dirPath = System.IO.Path.GetDirectoryName(dirPath);
        }

        [Prototype("id")]
        public JSValue Id => new JSString(filePath);

        JSValue exports;
        private bool IsMain;

        [Prototype("exports")]
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

        [Prototype("require")]
        public JSValue Require { get; set; }

        [Prototype("import")]
        public JSValue Import { get; set; }

        public Task<JSValue> ImportAsync(string name)
        {
            var result = Import.InvokeFunction(new Arguments(JSUndefined.Value, new JSString(name)));
            return (result as JSPromise).Task;
        }

        [Prototype("compile")]
        public JSValue Compile { get; set; }
        
        internal async Task InitAsync()
        {
            if (exports != null)
            {
                return;
            }
            exports = new JSObject();
            var result = this.Compile.InvokeFunction(in Arguments.Empty);
            if (result is JSPromise promise)
                await promise.Task;
        }
    }
}
