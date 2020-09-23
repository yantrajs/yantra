using System;
using System.Collections.Generic;
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

        internal override bool IsNullOrUndefined => true; 

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
            throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
        }

        public override JSValue Delete(uint key)
        {
            throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
        }

        public override JSValue this[KeyString name]
        {
            get => throw JSContext.Current.NewSyntaxError($"Cannot get property {name} of null");
            set => throw JSContext.Current.NewSyntaxError($"Cannot set property {name} of null");
        }

        public override JSValue this[uint key]
        {
            get => throw JSContext.Current.NewSyntaxError($"Cannot get property {key} of null");
            set => throw JSContext.Current.NewSyntaxError($"Cannot get property {key} of null");
        }


        //public override JSValue AddValue(JSValue value)
        //{
        //    switch(value)
        //    {
        //        case JSUndefined un:
        //            return JSNumber.NaN;
        //        case JSNull @null:
        //            return JSNumber.Zero;
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
                return JSBoolean.True;
            if (value.IsUndefined)
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (value.IsNull)
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw new NotImplementedException("null is not a function");
        }

        internal override IEnumerable<JSValue> AllElements => throw new NotImplementedException();
    }
}
