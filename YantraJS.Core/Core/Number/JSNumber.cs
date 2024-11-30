using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using System.Xml.Schema;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Primitive;
using YantraJS.ExpHelper;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{
    // [JSRuntime(typeof(JSNumberStatic), typeof(JSNumberPrototype))]
    [JSBaseClass("Object")]
    [JSFunctionGenerator("Number")]
    public sealed partial class JSNumber : JSPrimitive
    {

        internal readonly double value;


        private static readonly long positiveZeroBits = BitConverter.DoubleToInt64Bits(0.0);
        /// <summary>
        /// Determines if the given number is positive zero.
        /// </summary>
        /// <param name="value"> The value to test. </param>
        /// <returns> <c>true</c> if the value is positive zero; <c>false</c> otherwise. </returns>
        public static bool IsPositiveZero(double value)
        {
            return BitConverter.DoubleToInt64Bits(value) == positiveZeroBits;
        }

        private static readonly long negativeZeroBits = BitConverter.DoubleToInt64Bits(-0.0);

        /// <summary>
        /// Determines if the given number is negative zero.
        /// </summary>
        /// <param name="value"> The value to test. </param>
        /// <returns> <c>true</c> if the value is negative zero; <c>false</c> otherwise. </returns>
        public static bool IsNegativeZero(double value)
        {
            return BitConverter.DoubleToInt64Bits(value) == negativeZeroBits;
        }

        [JSExport("NaN")]
        public static JSNumber NaN = new JSNumber(double.NaN);

        public static JSNumber MinusOne = new JSNumber(-1);
        public static JSNumber Zero = new JSNumber(0d);
        public static JSNumber NegativeZero = new JSNumber(-0d);
        public static JSNumber One = new JSNumber(1d);
        public static JSNumber Two = new JSNumber(2d);

        [JSExport("POSITIVE_INFINITY")]
        public static JSNumber PositiveInfinity = new JSNumber(double.PositiveInfinity);

        [JSExport("NEGATIVE_INFINITY")]
        public static JSNumber NegativeInfinity = new JSNumber(double.NegativeInfinity);


        [JSExport("EPSILON")]
        public static readonly double Epsilon = 2.2204460492503130808472633361816E-16;

        [JSExport("MAX_SAFE_INTEGER")]
        public static readonly double MaxSafeInteger = 9007199254740991d;

        [JSExport("MAX_VALUE")]
        public static readonly double MaxValue = double.MaxValue;

        [JSExport("MIN_SAFE_INTEGER")]
        public static readonly double MinSafeInteger = -9007199254740991d;

        //Javascript considers double.Epsilon as MIN_VALUE and not .Net double.MinValue
        [JSExport("MIN_VALUE")]
        public static readonly double MinValue = double.Epsilon;
        

        public override bool IsNumber => true;

        public override JSValue TypeOf()
        {
            return JSConstants.Number;
        }

        protected override JSObject GetPrototype()
        {
            return (JSContext.Current[Names.Number] as JSFunction).prototype;
        }

        internal override PropertyKey ToKey(bool create = false)
        {
            var n = this.value;
            if (double.IsNaN(n))
                return KeyStrings.NaN;
            if (n == 0)
                return 0;
            if (n > 0 && ((uint)n) == n)
                return (uint)n;
            if (!create)
            {
                if (KeyStrings.TryGet(n.ToString(), out var k))
                    return k;
                return KeyStrings.undefined;
            }
            return KeyStrings.GetOrCreate(n.ToString());
        }

        public JSNumber(double value) : base()
        {
            //if (value > 0 && value < double.Epsilon)
            //{
            //    value = 0;
            //}
            this.value = value;
        }

        // public override int IntValue => (int)((long)value << 32) >> 32;

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
                // value = (int)((long)this.value << 32) >> 32;
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
            if (type == typeof(object))
            {
                value = this.value;
                return true;
            }
            if (type.IsAssignableFrom(typeof(JSNumber)))
            {
                value = this;
                return true;
            }
            value = null;
            return false;
        }

        public override string ToString()
        {
            if (double.IsPositiveInfinity(value))
                return JSConstants.Infinity.ToString();
            if (double.IsNegativeInfinity(value))
                return JSConstants.NegativeInfinity.ToString();
            if (value > 999999999999999.0)
                return value.ToString("g21");
            var v = value.ToString("g");
            return v;
        }

        public override string ToLocaleString(string format, CultureInfo culture)
        {
            return value.ToString(format,culture.NumberFormat);
        }

        public override string ToDetailString()
        {
            return value.ToString();
        }


        public static bool IsNaN(JSValue n)
        {
            return double.IsNaN(n.DoubleValue);
        }

        public override JSValue Negate()
        {
            return new JSNumber(-this.value);
        }

        public override JSValue AddValue(JSValue value)
        {
            value = value.IsObject ? value.ValueOf() : value;
            if (value is JSPrimitiveObject po)
                value = po.value;
            if (value is JSString @string)
                return new JSString(this.value + @string.ToString());
            if(value is JSObject @object)
                return new JSString(this.value + @object.StringValue);
            return new JSNumber(this.value + value.DoubleValue);
        }

        public override JSValue AddValue(double value)
        {
            return new JSNumber(this.value + value);
        }

        public override JSValue AddValue(string value)
        {
            return new JSString(this.value.ToString() + value);
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

        public override int GetHashCode()
        {
            return (int)value;
        }
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

        public override bool Equals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
            {
                if (double.IsNaN(this.value))
                    return false;
                return true;
            }
            switch (value)
            {
                case JSNumber number:
                    if (double.IsNaN(this.value) || double.IsNaN(number.value))
                        return false;
                    if(this.value == number.value)
                        return true;
                    return false;
                case JSString @string
                    when (this.value == @string.DoubleValue):
                    return true;
                case JSNull _
                    when (this.value == 0D):
                    return true;
                case JSBoolean boolean
                    when (this.value == (boolean._value ? 1D : 0D)):
                    return true;
            }
            // Added for this TC ExpressionTests.cs Assert.AreEqual(true, Evaluate("2 == [2]"));
            if (this.ToString() == value.ToString())
                return true;
            return false;
        }

        public override bool StrictEquals(JSValue value)
        {
            
            if (object.ReferenceEquals(this, value)) {
                if (double.IsNaN(this.value))
                    return false;
                return true;
            }
            if (value is JSNumber n)
            {
                if (double.IsNaN(this.value) || double.IsNaN(n.value))
                    return false;
                if (this.value == n.value)
                    return true;
            }
            return false;
        }

        public override bool SameValueZero(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
            {
                return true;
            }
            if (value is JSNumber n)
            {
                if (double.IsNaN(this.value) && double.IsNaN(n.value))
                    return true;
                if (this.value == n.value)
                    return true;
            }
            return false;
        }

        public override bool EqualsLiteral(double value)
        {
            return this.value == value;
        }

        public override bool EqualsLiteral(string value)
        {
            return this.value.ToString() == value || this.value == NumberParser.CoerceToNumber(value);
        }

        public override bool StrictEqualsLiteral(double value)
        {
            return this.value == value;
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
