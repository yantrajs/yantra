using System;
using System.Linq;

namespace WebAtoms.CoreJS.Core
{
    public sealed class JSNull : JSValue
    {
        private JSNull(): base(null)
        {

        }

        public override JSValue TypeOf()
        {
            return JSConstants.Object;
        }

        public static JSValue Value = new JSNull();

        public override string ToString()
        {
            return "null";
        }

        public override bool BooleanValue => false;

        public override double DoubleValue => 0D;

        public override bool IsNull => true;

        internal override KeyString ToKey()
        {
            return KeyStrings.@null;
        }

        public override bool Equals(object obj)
        {
            return obj is JSNull;
        }

        public override JSValue Delete(KeyString key)
        {
            throw JSContext.Current.NewTypeError($"Unable to delete {key} of null");
        }

        public override JSValue Delete(uint key)
        {
            throw JSContext.Current.NewTypeError($"Unable to delete {key} of null");
        }


        //public override JSValue AddValue(JSValue value)
        //{
        //    switch(value)
        //    {
        //        case JSUndefined un:
        //            return JSContext.Current.NaN;
        //        case JSNull @null:
        //            return JSContext.Current.Zero;
        //        case JSNumber n:
        //            return n;
        //    }
        //    return new JSString("null" + value.ToString());
        //}

        //public override JSValue AddValue(double value)
        //{
        //    return new JSNumber(value);
        //}

        //public override JSValue AddValue(string value)
        //{
        //    return new JSString("null" + value);
        //}

        public override JSBoolean Equals(JSValue value)
        {
            if (value.IsNull)
                return JSContext.Current.True;
            if (value.IsUndefined)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (value.IsNull)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw new NotImplementedException("null is not a function");
        }

    }
}
