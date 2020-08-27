using System;
using System.Linq;

namespace WebAtoms.CoreJS.Core
{
    public sealed class JSNull : JSValue
    {
        private JSNull(): base(null)
        {

        }

        public static JSNull Value = new JSNull();

        public override JSValue this[JSValue key] {
            get => throw new InvalidOperationException($"Cannot get {key} of null");
            set => throw new InvalidOperationException($"Cannot set {key} of null");
        }

        public override string ToString()
        {
            return "null";
        }

        public override bool Equals(object obj)
        {
            return obj is JSNull;
        }

        public override JSValue AddValue(JSValue value)
        {
            switch(value)
            {
                case JSUndefined un:
                    return JSContext.Current.NaN;
                case JSNull @null:
                    return JSContext.Current.Zero;
                case JSNumber n:
                    return n;
            }
            return new JSString("null" + value.ToString());
        }

        public override JSValue AddValue(double value)
        {
            return new JSNumber(value);
        }

        public override JSValue AddValue(string value)
        {
            return new JSString("null" + value);
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (value is JSNull)
                return JSContext.Current.True;
            if (value is JSUndefined)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (value is JSNull)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue, JSArray args)
        {
            throw new NotImplementedException("null is not a function");
        }
    }
}
