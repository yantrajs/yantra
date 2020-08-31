using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
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

        public static JSValue Substring(JSValue t, JSArguments a) 
        {
            var j = t as JSString;
            if (j == null)
                return JSUndefined.Value;
            var start = a[0];
            if (start is JSUndefined)
                return t;
            var length = a[1];
            if (length is JSUndefined)
                return new JSString(j.value.Substring(start.IntValue));
            return new JSString(j.value.Substring(start.IntValue, length.IntValue));
        }

        internal static JSFunction Create()
        {
            var r = new JSFunction(JSFunction.empty);
            var p = r.prototype;

            p.DefineProperty(KeyStrings.length, JSProperty.Property(
                (t, a) => new JSNumber(t.Length),
                (t, a) => a[0]));
            
            p.DefineProperty(KeyStrings.toString, JSProperty.Function((t, a) => t));

            var substr = JSProperty.Function(Substring);
            p.DefineProperty(KeyStrings.GetOrCreate("substr"), substr);
            p.DefineProperty(KeyStrings.GetOrCreate("substring"), substr);

            p.DefineProperties(JSProperty.Function(KeyStrings.toString, (t, a) => t));
            return r;
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

        public override JSValue InvokeFunction(JSValue thisValue, JSArguments args)
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
    }
}
