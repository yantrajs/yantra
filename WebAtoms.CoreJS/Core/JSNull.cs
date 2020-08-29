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

        internal override JSBoolean Less(JSValue value)
        {
            switch(value)
            {
                case JSNumber n:
                    if (0D < n.value)
                        return JSContext.Current.True;
                    break;
                case JSString str:
                    if (0D < str.DoubleValue)
                        return JSContext.Current.True;
                    break;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean LessOrEqual(JSValue value)
        {
            switch (value)
            {
                case JSNull @null:
                    return JSContext.Current.True;
                case JSString str:
                    if (0 <= str.DoubleValue)
                        return JSContext.Current.True;
                    break;
                case JSNumber n:
                    if (0 <= n.value)
                        return JSContext.Current.True;
                    break;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean Greater(JSValue value)
        {
            switch (value)
            {
                case JSString str:
                    if (0D > str.DoubleValue)
                        return JSContext.Current.True;
                    break;
                case JSNumber n:
                    if (0D > n.value)
                        return JSContext.Current.True;
                    break;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean GreaterOrEqual(JSValue value)
        {
            switch (value)
            {
                case JSNull @null:
                    return JSContext.Current.True;
                case JSString str:
                    if (0D >= str.DoubleValue)
                        return JSContext.Current.True;
                    break;
                case JSNumber n:
                    if (0D >= n.value)
                        return JSContext.Current.True;
                    break;
            }
            return JSContext.Current.False;
        }
    }
}
