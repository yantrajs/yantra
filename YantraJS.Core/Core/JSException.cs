using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace YantraJS.Core
{
    public class JSException: Exception
    {

        public override string Message
        {
            get {
                if (this.Error is JSError error)
                {
                    return error[KeyStrings.message].ToString();
                }
                return this.Error.ToString();
            }
        }

        public JSValue Error { get; internal set; }

        internal protected JSException With(JSValue error)
        {
            this.Error = error;
            return this;
        }

        private List<(StringSpan target, string file, int line, int column)> trace
            = new List<(StringSpan target, string file, int line, int column)>();

        public JSException(
                    JSValue message,
                    [CallerMemberName] string function = null,
                    [CallerFilePath] string filePath = null,
                    [CallerLineNumber] int line = 0) : base()
        {
            if (function != null)
            {
                this.trace.Add((function, filePath ?? "Unknown", line, 1));
            }
            Error = message;
        }

        public JSException(
            string message, 
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0): base(message)
        {
            if (function != null)
            {
                this.trace.Add((function, filePath ?? "Unknown", line, 1));
            }
            Error = new JSError(this, message);
        }

        public JSException(
            string message, 
            JSObject prototype,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0) : base(message)
        {
            if (function != null)
            {
                this.trace.Add((function, filePath ?? "Unknown", line, 1));
            }

            Error = new JSError(this, prototype);
        }


        public JSValue JSStackTrace
        {
            get
            {
                var sb = new StringBuilder();

                sb.AppendLine(this.Message);

                if (trace.Count > 0)
                {
                    var f = trace[0];
                    sb.AppendLine($"    at {f.target}:{f.file}:{f.line},{f.column}");
                }

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
                    trace.Add((fx, file, top.Line, top.Column));
                    top = top.Parent;
                }
                //var walker = JSContext.Current.StackWalker;
                //while (walker.MoveNext())
                //{
                //    ref var top = ref walker.Current;
                //    var fx = top.Function;
                //    var file = top.FileName;
                //    if (fx.IsNullOrWhiteSpace())
                //    {
                //        fx = "native";
                //    }
                //    if (string.IsNullOrWhiteSpace(file))
                //    {
                //        file = "file";
                //    }
                //    sb.AppendLine($"    at {fx}:{file}:{top.Line},{top.Column}");
                //    trace.Add((fx, file, top.Line, top.Column));
                //}
                return new JSString(sb.ToString());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Throw(JSValue value)
        {
            throw value is JSError jse ? jse.Exception : new JSException(value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue ThrowSyntaxError(string value)
        {
            throw JSContext.Current.NewSyntaxError(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSFunction ThrowNotFunction(JSValue value)
        {
            throw JSContext.Current.NewTypeError($"{value} is not a function");
        }

        public static JSException FromValue(JSValue value)
        {
            if (value is  JSError error)
            {
                return error.Exception;
            }
            var ex = new JSException(value.ToString());
            return ex;
        }


        public static JSException From(Exception exception)
        {
            if (exception.InnerException is JSException jse)
            {
                return jse;
            }
            if (exception is JSException jse2)
                return jse2;
            var error = new JSException(exception.InnerException?.ToString() ?? exception.ToString());
            return error;
        }

        public static JSValue ErrorFrom(Exception exception)
        {
            if (exception.InnerException is JSException jse)
            {
                return jse.Error;
            }
            if (exception is JSException jse2)
            {
                return jse2.Error;
            }
            var error = new JSException(exception.InnerException?.Message ?? exception.Message);
            return error.Error;
        }

        public override string StackTrace
        {
            get
            {
                var sb = new StringBuilder();
                foreach(var item in trace)
                {
                    sb.Append("at ");
                    sb.Append(item.target);
                    sb.Append(" in ");
                    sb.Append(item.file);
                    sb.Append(":line ");
                    sb.Append(item.line);
                    // sb.Append(" , ");
                    // sb.Append(item.column + 1);
                    sb.AppendLine();
                }

                // add internal stack..
                if (this.Error is JSError error)
                {
                    sb.AppendLine(error.Stack);
                }

                return sb.ToString();
            }
        }

        //public override string Message => 
        //    this.Stack != null 
        //    ? $"{Stack}{base.Message}"
        //    : base.Message;

        //public override string ToString()
        //{
        //    if (this.Stack != null)
        //    {
        //        return $"{Error}{Stack}{base.ToString()}";
        //    }
        //    return base.ToString();
        //}



    }
}
