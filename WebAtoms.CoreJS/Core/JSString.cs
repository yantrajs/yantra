using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public class JSString : JSValue
    {
        internal readonly string value;
        KeyString _keyString = new KeyString(null,0);

        public override double DoubleValue => NumberParser.CoerceToNumber(value);

        internal KeyString KeyString => _keyString.Value != null
            ? _keyString
            : (_keyString = KeyStrings.GetOrCreate(this.value));

        public JSString(string value): base(JSContext.Current.StringPrototype)
        {
            this.value = value;
        }

        internal JSString(string value, JSValue p) : base(p)
        {
            this.value = value;
        }

        public JSString(string value, KeyString keyString) : this(value)
        {
            this._keyString = keyString;
        }

        public static implicit operator KeyString(JSString value)
        {
            return value.ToString();
        }

        

        public override string ToString()
        {
            return value;
        }

        public override string ToDetailString()
        {
            return value;
        }

        public override int Length => value.Length;

        [Prototype("substring")]
        public static JSValue Substring(JSValue t,params JSValue[] a) 
        {
            var j = t as JSString;
            if (j == null)
                return JSUndefined.Value;
            if (!a.TryGetAt(0, out var start))
                return start;
            if (!a.TryGetAt(1, out var length))
                return new JSString(j.value.Substring(start.IntValue));
            return new JSString(j.value.Substring(start.IntValue, length.IntValue));
        }

        [Prototype("substr")]
        public static JSValue Substr(JSValue t,params JSValue[] a)
        {
            return Substring(t, a);
        }

        [Prototype("toString")]
        public static JSValue ToString(JSValue t,params JSValue[] a)
        {
            return t;
        }

        public override JSValue AddValue(JSValue value)
        {
            return new JSString(this.value + value.ToString());
        }

        public override JSValue AddValue(double value)
        {
            return new JSString(this.value + value.ToString());
        }

        public override JSValue AddValue(string value)
        {
            return new JSString(this.value + value);
        }

        public override bool Equals(object obj)
        {
            if (obj is JSString v)
                return this.value == v.value;
            return base.Equals(obj);
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            switch (value)
            {
                case JSString strValue
                    when ((this.value == strValue.value)
                    || (this.DoubleValue == value.DoubleValue)):
                    return JSContext.Current.True;
                case JSNumber number
                    when ((this.DoubleValue == number.value)
                        || (this.value.CompareTo(number.value.ToString()) == 0)):
                    return JSContext.Current.True;
                case JSBoolean boolean
                    when (this.DoubleValue == (boolean._value ? 1D : 0D)):
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            if (value is JSString s)
                if (s.value == this.value)
                    return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw new NotImplementedException("string is not a function");
        }

        internal override JSBoolean Less(JSValue value)
        {
            switch (value)
            {
                case JSString strValue
                    when ((this.value.CompareTo(strValue.value) < 0)
                    || (this.DoubleValue < value.DoubleValue)):
                    return JSContext.Current.True;
                case JSNumber number
                    when ((this.DoubleValue < number.value)
                        || (this.value.CompareTo(number.value.ToString()) < 0)):
                        return JSContext.Current.True;
                case JSBoolean boolean
                    when (this.DoubleValue < (boolean._value ? 1D : 0D)):
                        return JSContext.Current.True;                     
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean LessOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            switch (value)
            {
                case JSString strValue
                    when ((this.value.CompareTo(strValue.value) <= 0)
                    || (this.DoubleValue <= value.DoubleValue)):
                    return JSContext.Current.True;
                case JSNumber number
                    when ((this.DoubleValue <= number.value)
                        || (this.value.CompareTo(number.value.ToString()) <= 0)):
                    return JSContext.Current.True;
                case JSBoolean boolean
                    when (this.DoubleValue <= (boolean._value ? 1D : 0D)):
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean Greater(JSValue value)
        {
            switch (value)
            {
                case JSString strValue
                    when ((this.value.CompareTo(strValue.value) > 0)
                    || (this.DoubleValue > value.DoubleValue)):
                    return JSContext.Current.True;
                case JSNumber number
                    when ((this.DoubleValue > number.value)
                        || (this.value.CompareTo(number.value.ToString()) > 0)):
                    return JSContext.Current.True;
                case JSBoolean boolean
                    when (this.DoubleValue > (boolean._value ? 1D : 0D)):
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean GreaterOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            switch (value)
            {
                case JSString strValue
                    when ((this.value.CompareTo(strValue.value) >= 0)
                    || (this.DoubleValue > value.DoubleValue)):
                    return JSContext.Current.True;
                case JSNumber number
                    when ((this.DoubleValue > number.value)
                        || (this.value.CompareTo(number.value.ToString()) >= 0)):
                    return JSContext.Current.True;
                case JSBoolean boolean
                    when (this.DoubleValue >= (boolean._value ? 1D : 0D)):
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        [GetProperty("length")]
        internal static JSValue GetLength(JSValue t,params JSValue[] a)
        {
            return new JSNumber(((JSString)t).value.Length);
        }

        [SetProperty("length")]
        internal static JSValue SetLength(JSValue t,params JSValue[] a)
        {
            return a[0];
        }

    }
}
