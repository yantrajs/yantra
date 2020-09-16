using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using System.Xml.Schema;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    [JSRuntime(typeof(JSNumberStatic), typeof(JSNumberPrototype))]
    public sealed class JSNumber : JSPrimitive
    {

        internal readonly double value;

        public static JSNumber NaN = new JSNumber(double.NaN);
        public static JSNumber Zero = new JSNumber(0d);
        public static JSNumber One = new JSNumber(1d);
        public static JSNumber Two = new JSNumber(2d);


        [Static("EPSILON")]
        public static readonly double Epsilon = double.Epsilon;

        [Static("NAN")]
        public static readonly double JSNaN = double.NaN;

        [Static("MAX_SAFE_INTEGER")]
        public static readonly double MaxSafeInteger = 9007199254740991d;

        [Static("MAX_VALUE")]
        public static readonly double MaxValue = double.MaxValue;

        [Static("MIN_SAFE_INTEGER")]
        public static readonly double MinSafeInteger = -9007199254740991d;

        [Static("MIN_VALUE")]
        public static readonly double MinValue = double.MinValue;

        [Static("POSITIVE_INFINITY")]
        public static readonly double PositiveInfinity = double.PositiveInfinity;

        [Static("NEGATIVE_INFINITY")]
        public static readonly double NegativeInfinity = double.NegativeInfinity;

        public override bool IsNumber => true;

        public override JSValue TypeOf()
        {
            return JSConstants.Number;
        }

        protected override JSObject GetPrototype()
        {
            return JSContext.Current.NumberPrototype;
        }

        internal override KeyString ToKey()
        {
            var n = this.value;
            if (double.IsNaN(n))
                return KeyStrings.NaN;
            if (n == 0)
                return new KeyString(null, 0);
            if (n > 0 && ((int)n) == n)
                return new KeyString(null, (uint)n);
            return KeyStrings.GetOrCreate(n.ToString());
        }

        public JSNumber(double value): base()
        {
            this.value = value;
        }

        public override int IntValue => (int)value;

        public override double DoubleValue => value;

        public override bool BooleanValue => double.IsNaN(value)  ? false : value != 0;

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
                case JSNull @null
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

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw JSContext.Current.NewTypeError($"{this.value} is not a function");
        }

    }
}
