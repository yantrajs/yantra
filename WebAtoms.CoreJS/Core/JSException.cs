using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace WebAtoms.CoreJS.Core
{
    public class JSException: Exception
    {

        public JSValue Error { get; }

        public JSValue Stack { get; }

        private List<(string target, string file, int line, int column)> trace
            = new List<(string target, string file, int line, int column)>();

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
            Stack = Capture();
            Error = new JSError(new JSString(message), Stack);
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
            Stack = Capture();
            Error = new JSError(new JSString(message), Stack);
            Error.prototypeChain = prototype;
        }

        public JSException(
            JSValue message) : base(message.ToString())
        {
            Stack = Capture();
            if (message is JSError error) {
                error[KeyStrings.stack] = Stack;
            }
            else
            {
                Error = new JSError(message, Stack);
            }
        }

        private JSValue Capture()
        {
            var sb = new StringBuilder();
            var top = JSContext.Current.Scope.Top;
            if (trace.Count > 0)
            {
                var f = trace[0];
                sb.AppendLine($"    at {f.target}:{f.file}:{f.line},{f.column}");
            }
            while (top != null)
            {
                var fx = top.Function;
                var file = top.FileName;
                if (string.IsNullOrWhiteSpace(fx))
                {
                    fx = "native";
                }
                if (string.IsNullOrWhiteSpace(file))
                {
                    file = "file";
                }
                sb.AppendLine($"    at {fx}:{file}:{top.Position.Line},{top.Position.Column}");
                trace.Add((fx, file, top.Position.Line, top.Position.Column));
                top = top.Parent;
            }
            return new JSString(sb.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Throw(JSValue value)
        {
            throw new JSException(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSFunction ThrowNotFunction(JSValue value)
        {
            throw JSContext.Current.NewTypeError($"{value} is not a function");
        }

        public static JSException From(Exception exception)
        {
            if (exception.InnerException is JSException jse)
            {
                return jse;
            }
            var error = new JSString(exception.InnerException?.Message ?? exception.Message);
            return (new JSException(error));
        }

        public static JSValue ErrorFrom(Exception exception)
        {
            if (exception.InnerException is JSException jse)
            {
                return jse.Error;
            }
            var error = new JSString(exception.InnerException?.Message ?? exception.Message);
            return (new JSException(error)).Error;
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
