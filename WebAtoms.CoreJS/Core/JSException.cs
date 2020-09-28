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

        public JSException(string message): base(message)
        {
            Error = new JSError(new JSString(message), JSUndefined.Value);
            Stack = Capture();
        }

        public JSException(string message, JSObject prototype) : base(message)
        {
            Error = new JSError(new JSString(message), JSUndefined.Value);
            Error.prototypeChain = prototype;
            Stack = Capture();
        }

        public JSException(JSValue message) : base(message.ToString())
        {
            Error = new JSError(message, JSUndefined.Value);
            Stack = Capture();
        }

        private JSValue Capture()
        {
            var sb = new StringBuilder();
            var top = JSContext.Current.Scope.Top;
            while (top != null)
            {
                sb.AppendLine($"{top.Function} {top.FileName} {top.Position.Line},{top.Position.Column}");
                var fx = top.Function;
                var file = top.FileName;
                if (string.IsNullOrWhiteSpace(fx))
                {
                    fx = "native";
                }
                if(string.IsNullOrWhiteSpace(file))
                {
                    file = "file";
                }
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
            throw new JSException($"{value} is not a function");
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
