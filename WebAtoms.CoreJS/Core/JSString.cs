using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSString : JSValue
    {
        internal readonly string value;
        KeyString _keyString = new KeyString(null,0);

        public override double DoubleValue => NumberParser.CoerceToNumber(value);

        public override bool BooleanValue => value.Length > 0;

        internal override KeyString ToKey()
        {
            return _keyString.Value != null
                ? _keyString
                : (_keyString = KeyStrings.GetOrCreate(this.value));
        }


        public JSString(string value): base(JSContext.Current.StringPrototype)
        {
            this.value = value;
        }

        internal JSString(string value, JSObject prototype) : base(prototype)
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

        public override JSValue this[uint key] { 
            get
            {
                if (key >= this.value.Length)
                    return JSUndefined.Value;
                return new JSString(new string(this.value[(int)key],1));
            } 
            set => base[key] = value; 
        }

        public override int Length => value.Length;

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


    }
}
