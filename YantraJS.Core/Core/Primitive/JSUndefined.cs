using System;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.Core
{
    public sealed class JSUndefined : JSValue
    {
        private JSUndefined():base(null)
        {

        }

        public static JSValue Value = new JSUndefined();

        internal override KeyString ToKey(bool create = true)
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
            set => throw JSContext.Current.NewSyntaxError($"Cannot set property {name} of undefined");
        }

        public override JSValue this[uint key]
        {
            get => throw JSContext.Current.NewSyntaxError($"Cannot get property {key} of undefined");
            set => throw JSContext.Current.NewSyntaxError($"Cannot set property {key} of undefined");
        }

        internal override JSFunctionDelegate GetMethod(in KeyString key)
        {
            throw JSContext.Current.NewSyntaxError($"Cannot get property {key} of undefined");
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

        public override JSValue CreateInstance(in Arguments a)
        {
            throw JSContext.Current.NewTypeError("cannot create instance of undefined");
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw JSContext.Current.NewTypeError("undefined is not a function");
        }

        internal override IElementEnumerator GetElementEnumerator()
        {
            throw JSContext.Current.NewTypeError("undefined is not iterable");
        }

        public override bool ConvertTo(Type type, out object value)
        {
            if (type.IsAssignableFrom(typeof(JSUndefined)))
            {
                value = this;
                return true;
            }
            value = null;
            return !type.IsValueType;
        }


    }
}
