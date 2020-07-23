using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public sealed class JSNumber : JSValue
    {

        internal readonly double value;

        public const double MaxSafeInteger = 9007199254740991d;

        public const double MinSafeInteger = -9007199254740991d;

        internal JSNumber(double value): base(JSContext.Current.NumberPrototype)
        {
            this.value = value;
        }

        internal JSNumber(double value, JSValue jsPrototype) : base(jsPrototype)
        {
            this.value = value;
        }

        public override int IntValue => (int)value;

        public override double DoubleValue => value;

        public override string ToString()
        {
            return value.ToString();
        }

        public static JSValue IsFinite(JSValue t, JSArray a)
        {
            if (a._length > 0 && a[0] is JSNumber n) {
                if (n.value != double.NaN && n.value > Double.NegativeInfinity && n.value < double.PositiveInfinity)
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        public static JSValue IsInteger(JSValue t, JSArray a)
        {
            if (a._length > 0 && a[0] is JSNumber n) { 
                var v = n.value;
                if(((int)v) == v) 
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        public static JSValue IsNaN(JSValue t, JSArray a)
        {
            if (a._length > 0 && a[0] is JSNumber n)
            {
                if (n.value == double.NaN)
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        public static JSValue IsSafeInteger(JSValue t, JSArray a)
        {
            if (a._length > 0 && a[0] is JSNumber n)
            {
                var v = n.value;
                if (v > MinSafeInteger && v < MaxSafeInteger)
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        public static JSNumber ParseFloat(JSValue t, JSArray a)
        {
            var nan = JSContext.Current.NaN;
            if (a._length > 0)
            {
                var p = a[0];
                switch(p)
                {
                    case JSNumber n:
                        return n;
                    case JSNull _:
                    case JSUndefined _:
                        return nan;
                }
                var text = p.JSTrim();
                if (text.Length > 0)
                {
                    if (double.TryParse(text, out var d))
                        return new JSNumber(d);
                }
            }
            return nan;
        }


        public static JSNumber ParseInt(JSValue t, JSArray a)
        {
            var nan = JSContext.Current.NaN;
            if (a._length > 0)
            {
                var p = a[0];
                switch (p)
                {
                    case JSNumber n:
                        return n;
                    case JSNull _:
                    case JSUndefined _:
                        return nan;
                }
                var text = p.JSTrim();
                if (text.Length > 0)
                {
                    if (a._length > 1)
                    {
                        var b = ParseInt(t, JSArguments.From(a[1]));
                        if (!double.IsNaN(b.value) && b.value > 0)
                        {
                            if (b.value != 10)
                            {
                                throw new NotSupportedException();
                            }
                        }
                    }
                    if (int.TryParse(text, out var d))
                        return new JSNumber(d);
                }
            }
            return nan;
        }

        public static JSString ToExponential(JSValue t, JSArray a)
        {
            var p = t;
            if (!(p is JSNumber n))
                throw JSContext.Current.TypeError($"Number.prototype.toExponential requires that 'this' be a Number");
            return new JSString(n.value.ToString("E"));
        }


        internal static JSFunction Create()
        {
            var r = new JSFunction(JSFunction.empty, "Number");
            var prototype = r.prototype;
            r.DefineProperty("EPSILON", JSProperty.Property(
                new JSNumber(double.Epsilon, prototype),
                JSPropertyAttributes.Property | JSPropertyAttributes.Readonly));
            r.DefineProperty("NAN", JSProperty.Property(
                new JSNumber(double.NaN, prototype),
                JSPropertyAttributes.Property | JSPropertyAttributes.Readonly));
            r.DefineProperty("MAX_SAFE_INTEGER", JSProperty.Property(
                new JSNumber(MaxSafeInteger, prototype),
                JSPropertyAttributes.Property | JSPropertyAttributes.Readonly));
            r.DefineProperty("MAX_VALUE", JSProperty.Property(
                new JSNumber(double.MaxValue, prototype),
                JSPropertyAttributes.Property | JSPropertyAttributes.Readonly));
            r.DefineProperty("MIN_SAFE_INTEGER", JSProperty.Property(
                new JSNumber(MinSafeInteger, prototype),
                JSPropertyAttributes.Property | JSPropertyAttributes.Readonly));
            r.DefineProperty("MIN_VALUE", JSProperty.Property(
                new JSNumber(double.MinValue, prototype),
                JSPropertyAttributes.Property | JSPropertyAttributes.Readonly));
            r.DefineProperty("NEGATIVE_INFINITY", JSProperty.Property(
                new JSNumber(double.NegativeInfinity, prototype),
                JSPropertyAttributes.Property | JSPropertyAttributes.Readonly));
            r.DefineProperty("POSITIVE_INFINITY", JSProperty.Property(
                new JSNumber(double.PositiveInfinity, prototype),
                JSPropertyAttributes.Property | JSPropertyAttributes.Readonly));

            r.DefineProperty("isFinite", JSProperty.Function(IsFinite));

            r.DefineProperty("isInteger", JSProperty.Function(IsInteger));

            r.DefineProperty("isNaN", JSProperty.Function(IsNaN));

            r.DefineProperty("isSafeInteger", JSProperty.Function(IsSafeInteger));

            r.DefineProperty("parseFloat", JSProperty.Function(ParseFloat));

            r.DefineProperty("parseInt", JSProperty.Function(ParseInt));

            prototype.DefineProperty("toExponential", JSProperty.Function(ToExponential));

            return r;
        }
    }
}
