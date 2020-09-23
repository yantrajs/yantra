using System;
using System.Collections.Generic;
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

        public override JSValue TypeOf()
        {
            return JSConstants.Undefined;
        }

        public override bool BooleanValue => false;

        public override double DoubleValue => double.NaN;

        public override bool IsUndefined => true;

        internal override bool IsNullOrUndefined => true;

        public override string ToString()
        {
            return "undefined";
        }

        public override JSValue this[KeyString name] {
            get => throw JSContext.Current.NewSyntaxError($"Cannot get property {name} of undefined");
            set => throw JSContext.Current.NewSyntaxError($"Cannot get property {name} of undefined");
        }

        public override JSValue this[uint key]
        {
            get => throw JSContext.Current.NewSyntaxError($"Cannot get property {key} of undefined");
            set => throw JSContext.Current.NewSyntaxError($"Cannot get property {key} of undefined");
        }

        public override JSValue Delete(KeyString key)
        {
            throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
        }

        public override JSValue Delete(uint key)
        {
            throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
        }

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
            if (value.IsUndefined)
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw new NotImplementedException("undefined is not a function");
        }

        internal override IEnumerable<JSValue> AllElements => throw new NotImplementedException();

    }
}
