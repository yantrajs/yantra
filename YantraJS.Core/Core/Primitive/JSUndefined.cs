using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace YantraJS.Core
{
    public sealed class JSUndefined : JSValue
    {
        private JSUndefined():base(null)
        {

        }

        public static JSValue Value = new JSUndefined();

        internal override PropertyKey ToKey(bool create = true)
        {
            return KeyStrings.undefined;
        }

        public override JSValue TypeOf()
        {
            return JSConstants.Undefined;
        }

        public override bool BooleanValue => false;

        public override double DoubleValue => double.NaN;

        public override uint UIntValue => 0;

        public override int IntegerValue => 0;

        public override int IntValue => 0;
        //public override bool IsUndefined => true;

        //internal override bool IsNullOrUndefined => true;

        public override string ToString()
        {
            return "undefined";
        }

        public override JSValue this[KeyString name] {
            get => throw JSContext.Current.NewTypeError($"Cannot get property {name} of undefined");
            set => throw JSContext.Current.NewTypeError($"Cannot set property {name} of undefined");
        }

        public override JSValue this[uint key]
        {
            get => throw JSContext.Current.NewTypeError($"Cannot get property {key} of undefined");
            set => throw JSContext.Current.NewTypeError($"Cannot set property {key} of undefined");
        }

        internal override JSFunctionDelegate GetMethod(in KeyString key)
        {
            throw JSContext.Current.NewTypeError($"Cannot get property {key} of undefined");
        }

        public override JSValue Delete(in KeyString key)
        {
            throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
        }

        public override JSValue Delete(uint key)
        {
            throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
        }

        public override bool Equals(JSValue value)
        {
            return value.IsNullOrUndefined;
            //if (value.IsUndefined)
            //    return true;
            //return false;
        }

        public override bool StrictEquals(JSValue value)
        {
            return ReferenceEquals(this, value);
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            throw JSContext.Current.NewTypeError("cannot create instance of undefined");
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw JSContext.Current.NewTypeError("undefined is not a function", null);
        }

        public override IElementEnumerator GetElementEnumerator()
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

        public override string ToLocaleString(string format, CultureInfo culture)
        {
            return "";
        }
    }
}
