using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace YantraJS.Core
{
    public sealed class JSNull : JSValue
    {
        private JSNull(): base(JSValueType.Null, default)
        {

        }

        // public override JSValue TypeOf()
        // {
        //     return JSConstants.Object;
        // }

        public static JSValue Value = new JSNull();

        public override string ToString()
        {
            return "null";
        }

        public override bool BooleanValue => false;

        public override double DoubleValue => 0D;

        public override uint UIntValue => 0;

        public override int IntegerValue => 0;

        public override int IntValue => 0;

        //public override bool IsNull => true;

        //internal override bool IsNullOrUndefined => true; 

        public override JSValue Negate()
        {
            return JSNumber.NegativeZero;
        }

        internal override PropertyKey ToKey(bool create = false)
        {
            return KeyString.@null;
        }

        public override bool Equals(object obj)
        {
            return obj is JSNull;
        }

        public override JSValue Delete(in KeyString key)
        {
            throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
        }

        public override JSValue Delete(uint key)
        {
            throw JSContext.Current.NewTypeError(JSError.Cannot_convert_undefined_or_null_to_object);
        }

        //public override JSValue this[KeyString name]
        //{
        //    get => throw JSContext.Current.NewTypeError($"Cannot get property {name} of null");
        //    set => throw JSContext.Current.NewTypeError($"Cannot set property {name} of null");
        //}

        //public override JSValue this[uint key]
        //{
        //    get => throw JSContext.Current.NewTypeError($"Cannot get property {key} of null");
        //    set => throw JSContext.Current.NewTypeError($"Cannot get property {key} of null");
        //}

        protected internal sealed override JSValue GetValue(KeyString key, JSValue receiver, bool throwError = true)
        {
            throw JSContext.Current.NewTypeError($"Cannot get property ${key.ToStringSpan()} of null");
        }

        protected internal sealed override bool SetValue(KeyString key, JSValue value, JSValue receiver, bool throwError = true)
        {
            throw JSContext.Current.NewTypeError($"Cannot set property ${key.ToStringSpan()} of null");
        }

        protected internal sealed override JSValue GetValue(JSSymbol key, JSValue receiver, bool throwError = true)
        {
            throw JSContext.Current.NewTypeError($"Cannot get property ${key.ToString()} of null");
        }

        protected internal sealed override bool SetValue(JSSymbol key, JSValue value, JSValue receiver, bool throwError = true)
        {
            throw JSContext.Current.NewTypeError($"Cannot set property ${key.ToString()} of null");
        }

        protected internal sealed override JSValue GetValue(uint key, JSValue receiver, bool throwError = true)
        {
            throw JSContext.Current.NewTypeError($"Cannot get property ${key} of null");
        }

        protected internal sealed override bool SetValue(uint key, JSValue value, JSValue receiver, bool throwError = true)
        {
            throw JSContext.Current.NewTypeError($"Cannot get property ${key} of null");
        }

        internal override JSFunctionDelegate GetMethod(in KeyString key)
        {
            throw JSContext.Current.NewTypeError($"Cannot get property {key} of null");
        }


        public override IElementEnumerator GetElementEnumerator()
        {
            throw JSContext.Current.NewTypeError("null is not iterable");
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

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(JSValue value)
        {
            if (value.IsNull)
                return true;
            if (value.IsUndefined)
                return true;
            return false;
        }

        public override bool StrictEquals(JSValue value)
        {
            return Object.ReferenceEquals(this, value);
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            throw JSContext.Current.NewTypeError("cannot create instance of null");
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException("null is not a function");
        }

        public override bool ConvertTo(Type type, out object value)
        {
            if (type.IsAssignableFrom(typeof(JSNull)))
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
