using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using System.Xml.Schema;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.ExpHelper;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    [JSRuntime(typeof(JSNumberStatic), typeof(JSNumberPrototype))]
    public sealed class JSNumber : JSPrimitive
    {

        internal readonly double value;

        [Static("NaN")]
        public static JSNumber NaN = new JSNumber(double.NaN);

        public static JSNumber MinusOne = new JSNumber(-1);
        public static JSNumber Zero = new JSNumber(0d);
        public static JSNumber NegativeZero = new JSNumber(-0d);
        public static JSNumber One = new JSNumber(1d);
        public static JSNumber Two = new JSNumber(2d);

        [Static("POSITIVE_INFINITY")]
        public static JSNumber PositiveInfinity = new JSNumber(double.PositiveInfinity);

        [Static("NEGATIVE_INFINITY")]
        public static JSNumber NegativeInfinity = new JSNumber(double.NegativeInfinity);


        [Static("EPSILON")]
        public static readonly double Epsilon = double.Epsilon;

        [Static("MAX_SAFE_INTEGER")]
        public static readonly double MaxSafeInteger = 9007199254740991d;

        [Static("MAX_VALUE")]
        public static readonly double MaxValue = double.MaxValue;

        [Static("MIN_SAFE_INTEGER")]
        public static readonly double MinSafeInteger = -9007199254740991d;

        [Static("MIN_VALUE")]
        public static readonly double MinValue = double.MinValue;

        public override bool IsNumber => true;

        public override JSValue TypeOf()
        {
            return JSConstants.Number;
        }

        protected override JSObject GetPrototype()
        {
            return JSContext.Current.NumberPrototype;
        }

        internal override KeyString ToKey(bool create = false)
        {
            var n = this.value;
            if (double.IsNaN(n))
                return KeyStrings.NaN;
            if (n == 0)
                return new KeyString(null, 0);
            if (n > 0 && ((int)n) == n)
                return new KeyString(null, (uint)n);
            if (!create)
            {
                if (KeyStrings.TryGet(n.ToString(), out var k))
                    return k;
                return KeyStrings.undefined;
            }
            return KeyStrings.GetOrCreate(n.ToString());
        }

        public JSNumber(double value): base()
        {
            this.value = value;
        }

        public override int IntValue => (int)value;

        public override double DoubleValue => value;

        public override bool BooleanValue => !double.IsNaN(value) && value != 0;

        public override long BigIntValue => (long)this.value;

        public override bool ConvertTo(Type type, out object value)
        {
            if(type == typeof(double))
            {
                value = this.value;
                return true;
            }
            if (type == typeof(float))
            {
                value = this.value;
                return true;
            }
            if (type == typeof(int)) {
                value = (int)this.value;
                return true;
            }
            if (type == typeof(long))
            {
                value = (long)this.value;
                return true;
            }
            if (type == typeof(ulong))
            {
                value = (ulong)this.value;
                return true;
            }
            if (type == typeof(bool))
            {
                value = this.value != 0;
                return true;
            }
            if (type == typeof(short))
            {
                value = (short)this.value;
                return true;
            }
            if (type == typeof(uint))
            {
                value = (uint)this.value;
                return true;
            }
            if (type == typeof(ushort))
            {
                value = (ushort)this.value;
                return true;
            }
            if (type == typeof(byte))
            {
                value = (byte)this.value;
                return true;
            }
            if (type == typeof(sbyte))
            {
                value = (sbyte)this.value;
                return true;
            }
            value = null;
            return false;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public override string ToDetailString()
        {
            return value.ToString();
        }


        public static bool IsNaN(JSValue n)
        {
            return double.IsNaN(n.DoubleValue);
        }

        public override JSValue AddValue(JSValue value)
        {
            if (value is JSString @string)
                return new JSString(this.value + @string.value);
            return new JSNumber(this.value + value.DoubleValue);
        }


        //public override JSValue AddValue(JSValue value)
        //{
        //    switch(value)
        //    {
        //        case JSUndefined u:
        //            return JSNumber.NaN;
        //        case JSNull n:
        //            return this;
        //        case JSNumber n1:
        //            var v = n1.value;
        //            if (double.IsNaN(v)
        //                || double.IsPositiveInfinity(v)
        //                || double.IsNegativeInfinity(v))
        //            {
        //                return n1;
        //            }
        //            return this.AddValue(v);
        //    }
        //    return new JSString(this.value.ToString() + value.ToString());
        //}

        //public override JSValue AddValue(double value)
        //{
        //    var v = this.value;
        //    if (double.IsNaN(v)
        //        || double.IsPositiveInfinity(v)
        //        || double.IsNegativeInfinity(v))
        //        return this;
        //    return new JSNumber(v + value);
        //}

        //public override JSValue AddValue(string value)
        //{
        //    return new JSString(this.value.ToString() + value);
        //}

        public override bool Equals(object obj)
        {
            if (obj is JSNumber n)
            {
                if (double.IsNaN(value) || double.IsNaN(n.value))
                    return false;
                return value == n.value;
            }
            return base.Equals(obj);
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return JSBoolean.True;
            switch (value)
            {
                case JSNumber number
                    when (this.value == number.value):
                    return JSBoolean.True;
                case JSString @string
                    when (this.value == @string.DoubleValue):
                    return JSBoolean.True;
                case JSNull _
                    when (this.value == 0D):
                    return JSBoolean.True;
                case JSBoolean boolean
                    when (this.value == (boolean._value ? 1D : 0D)):
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return JSBoolean.True;
            if (value is JSNumber n)
            {
                if (this.value == n.value)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"{this.value} is not a function");
        }

        internal override JSBoolean Is(JSValue value)
        {
            if(value is JSNumber number)
            {
                if (this.value == 0 || number.value == 0)
                {
                    return BitConverter.DoubleToInt64Bits(this.value) == BitConverter.DoubleToInt64Bits(number.value)
                        ? JSBoolean.True
                        : JSBoolean.False;
                } 
                
                if (double.IsNaN(this.value))
                    return double.IsNaN(number.value) ? JSBoolean.True : JSBoolean.False;
                if (this.value == number.value)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }
    }
}
