using System;
using System.Linq;

namespace WebAtoms.CoreJS.Core
{
    public sealed class JSUndefined : JSValue
    {
        private JSUndefined():base(null)
        {

        }

        public static JSValue Value = new JSUndefined();

        internal override KeyString ToKey()
        {
            return KeyStrings.undefined;
        }

        public override bool BooleanValue => false;

        public override bool IsUndefined => true;

        public override string ToString()
        {
            return "undefined";
        }

        public override JSValue AddValue(JSValue value)
        {
            switch(value)
            {
                case JSUndefined un:
                case JSNumber n:
                    return JSContext.Current.NaN;
            }
            return new JSString("undefined" + value.ToString());
        }

        public override JSValue AddValue(double value)
        {
            return JSContext.Current.NaN;
        }

        public override JSValue AddValue(string value)
        {
            return new JSString("undefined" + value);
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
            if (value is JSUndefined)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw new NotImplementedException("undefined is not a function");
        }

        internal override JSBoolean Less(JSValue value)
        {
            return JSContext.Current.False;
        }

        internal override JSBoolean LessOrEqual(JSValue value)
        {
            return JSContext.Current.False;
        }

        internal override JSBoolean Greater(JSValue value)
        {
            return JSContext.Current.False;
        }

        internal override JSBoolean GreaterOrEqual(JSValue value)
        {
            return JSContext.Current.False;
        }
    }
}
