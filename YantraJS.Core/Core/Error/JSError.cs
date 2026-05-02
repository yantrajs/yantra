using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Core
{


    [JSClassGenerator("Error")]
    public partial class JSError : JSObject
    {
        public string Message { get; private set; }

        public string Stack { get; private set; }

        private string CreateStack()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{this.ToString(Arguments.Empty)}");

            var top = JSContext.Current.Top;
            while (top != null)
            {
                // ref var top = ref walker.Current;
                var fx = top.Function;
                var file = top.FileName;
                if (fx.IsNullOrWhiteSpace())
                {
                    fx = "native";
                }
                if (string.IsNullOrWhiteSpace(file))
                {
                    file = "file";
                }
                sb.AppendLine($"    at {fx}:{file}:{top.Line},{top.Column}");
                top = top.Parent;
            }
            return sb.ToString();
        }

        protected JSError(JSPrototypeObject prototype): base(prototype)
        {

        }
        public JSError(
            JSPrototypeObject prototype,
            string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0
            ): this( prototype.PrototypeOrNewTrget)
        {
            this.Exception = new JSException(
                this,
                function: function,
                filePath: filePath,
                line: line);
            this.Message = message;
            this.Stack = this.CreateStack();
            this.FastAddValue(KeyString.message, message.Marshal(), JSPropertyAttributes.ConfigurableValue);
            this.FastAddValue(KeyString.stack, Stack.Marshal(), JSPropertyAttributes.ConfigurableValue);
        }
        public JSError(in Arguments a): this(a.NewTarget)
        {
            this.Exception = new JSException(
                this);
            var message = a[0]?.ToString() ?? "Internal Error";
            this.Message = message;
            this.Stack = this.CreateStack();
            this.FastAddValue(KeyString.message, message.Marshal(), JSPropertyAttributes.ConfigurableValue);
            this.FastAddValue(KeyString.stack, Stack.Marshal(), JSPropertyAttributes.ConfigurableValue);
        }

        [JSExport("toString")]
        public new JSValue ToString(in Arguments a)
        {
            var name = this.prototypeChain.@object[KeyString.constructor][KeyString.name];
            return new JSString($"{name}: {this.Message}");
        }

        public override string ToString()
        {
            return ToString(Arguments.Empty).ToString();
        }

        public override string ToDetailString()
        {
            return ToString(Arguments.Empty).ToString() + "\r\n" + this.Exception.JSStackTrace.ToString();
        }


        public const string Cannot_convert_undefined_or_null_to_object = "Cannot convert undefined or null to object";

        public const string Parameter_is_not_an_object = "Parameter is not an object";

        public JSException Exception { get; }

        //protected JSError( JSValue message, JSValue stack,  JSObject prototype) : base(prototype)
        //{
        //    this.DefineProperty(KeyString.message, JSProperty.Property(message, JSPropertyAttributes.ConfigurableValue));
        //    this.DefineProperty(KeyString.stack, JSProperty.Property(stack, JSPropertyAttributes.ConfigurableValue));
        //}

        //internal JSError(
        //    string message,
        //    [CallerMemberName] string function = null,
        //    [CallerFilePath] string filePath = null,
        //    [CallerLineNumber] int line = 0,
        //    JSObject prototype = null) : base(prototype)
        //{
        //    this.Exception = new JSException();
        //    this.FastAddValue(KeyString.message, ex.Message.Marshal(), JSPropertyAttributes.ConfigurableValue);
        //    this.FastAddValue(KeyString.stack, ex.JSStackTrace, JSPropertyAttributes.ConfigurableValue);
        //}

        internal protected JSError(
            JSException ex,
            JSPrototypeObject prototype) : base(prototype)
        {
            this.Exception = ex;
            ex.Error ??= this;
            this.FastAddValue(KeyString.message, ex.Message.Marshal(), JSPropertyAttributes.ConfigurableValue);
            this.FastAddValue(KeyString.stack, ex.JSStackTrace, JSPropertyAttributes.ConfigurableValue);
        }


        internal JSError(JSException ex, string msg) : this(JSContext.CurrentContext.Error_Prototype)
        {
            this.Exception = ex;
            ex.Error ??= this;
            this.Message = msg;
            this.FastAddValue(KeyString.message, msg.Marshal(), JSPropertyAttributes.ConfigurableValue);
            this.FastAddValue(KeyString.stack, ex.JSStackTrace, JSPropertyAttributes.ConfigurableValue);
        }

        public static JSValue From(Exception ex)
        {
            if(ex is JSException jse)
            {
                return jse.Error;
            }
            var je = new JSException(ex.Message + "\r\n" + ex.ToString());
            return je.Error;
        }
    }

    [JSClassGenerator("TypeError"), JSBaseClass("Error")]
    public partial class JSTypeError: JSError
    {
        public JSTypeError(in Arguments a): base(in a)
        {
            
        }

        public JSTypeError(JSPrototypeObject prototype,
            string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0) : base(prototype, message, function, filePath, line)
        {
        }

        public static string NotIterable(object name) => $"{name} is not iterable";

        public static string NotEntry(object name) => $"Iterator value {name} is an entry object";
    }
    [JSClassGenerator("SyntaxError"), JSBaseClass("Error")]
    public partial class JSSyntaxError: JSError
    {
        public JSSyntaxError(in Arguments a) : base(in a)
        {

        }


        public JSSyntaxError(JSPrototypeObject prototype,
            string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0) : base(prototype, message, function, filePath, line)
        {
        }
    }
    [JSClassGenerator("URIError"), JSBaseClass("Error")]
    public partial class JSURIError: JSError
    {
        public JSURIError(in Arguments a) : base(in a)
        {

        }

        public JSURIError(
            JSPrototypeObject prototype,
            string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0) : base(prototype, message, function, filePath, line)
        {
        }
    }
    [JSClassGenerator("RangeError"), JSBaseClass("Error")]
    public partial class JSRangeError: JSError
    {
        public JSRangeError(in Arguments a) : base(in a)
        {

        }

        public JSRangeError(JSPrototypeObject prototype, string message, [CallerMemberName] string function = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int line = 0) : base(prototype, message, function, filePath, line)
        {
        }
    }

    [JSClassGenerator("EvalError"), JSBaseClass("Error")]
    public partial class JSEvalError : JSError
    {
        public JSEvalError(in Arguments a) : base(in a)
        {

        }
        public JSEvalError(JSPrototypeObject prototype, string message, [CallerMemberName] string function = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int line = 0) : base(prototype, message, function, filePath, line)
        {
        }

    }
}
