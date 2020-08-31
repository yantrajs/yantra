using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WebAtoms.CoreJS.Core
{
    public class JSException: Exception
    {

        public JSValue Error { get; }

        public JSValue Stack { get; }

        public JSException(string message): base(message)
        {
            Error = new JSError(new JSString(message), JSUndefined.Value);
            Stack = Capture();
        }

        public JSException(string message, JSValue prototype) : base(message)
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
                top = top.Parent;
            }
            return new JSString(sb.ToString());
        }

        internal static void Throw(JSValue value)
        {
            throw new JSException(value);
        }

        public override string Message => 
            this.Stack != null 
            ? $"{Stack}{base.Message}"
            : base.Message;

        public override string ToString()
        {
            if (this.Stack != null)
            {
                return $"{Error}{Stack}{base.ToString()}";
            }
            return base.ToString();
        }



    }
}
