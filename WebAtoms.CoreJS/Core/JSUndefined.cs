using System;
using System.Linq;

namespace WebAtoms.CoreJS.Core
{
    public sealed class JSUndefined : JSValue
    {
        private JSUndefined():base(null)
        {

        }

        public static JSUndefined Value = new JSUndefined();

        public override JSValue this[JSValue key]
        {
            get => throw new InvalidOperationException($"Cannot get {key} of undefined");
            set => throw new InvalidOperationException($"Cannot set {key} of undefined");
        }

        public override string ToString()
        {
            return "undefined";
        }

        public override JSValue Add(JSValue value)
        {
            switch(value)
            {
                case JSUndefined un:
                case JSNumber n:
                    return JSContext.Current.NaN;
            }
            return new JSString("undefined" + value.ToString());
        }

        public override JSValue Add(double value)
        {
            return JSContext.Current.NaN;
        }

        public override JSValue Add(string value)
        {
            return new JSString("undefined" + value);
        }
    }
}
