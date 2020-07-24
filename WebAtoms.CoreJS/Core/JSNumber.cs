using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

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
                if (double.IsNaN(n.value))
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        public static JSValue IsSafeInteger(JSValue t, JSArray a)
        {
            if (a._length > 0 && a[0] is JSNumber n)
            {
                var v = n.value;
                if (v >= MinSafeInteger && v <= MaxSafeInteger)
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
                    int start = 0;
                    char ch;
                    bool hasDot = false;
                    bool hasE = false;
                    do {
                        ch = text[start];
                        if (char.IsDigit(ch))
                        {
                            start ++;
                            continue;
                        }
                        if (ch == '.')
                        {
                            if (!hasDot)
                            {
                                hasDot = true;
                                start++;
                                continue;
                            }
                            break;
                        }
                        if (ch == 'E' || ch == 'e')
                        {
                            if (!hasE)
                            {
                                hasE = true;
                                start++;
                                if (start < text.Length)
                                {
                                    var next = text[start];
                                    if (next == '+' || next == '-')
                                    {
                                        start++;
                                        continue;
                                    }
                                }
                                continue;
                            }
                            break;
                        }
                        break;
                    } while (start < text.Length);
                    if (text.Length > start)
                        text = text.Substring(0, start);
                    if (text.EndsWith("e+"))
                        text += "0";
                    if (text.EndsWith("e"))
                        text += "+0";
                    if (double.TryParse(text, out var d))
                    {
                        return new JSNumber(d);
                    }
                    return nan;
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
                    var radix = 10;
                    if (a._length > 2)
                    {
                        var a1 = a[1];
                        switch(a1)
                        {
                            case JSUndefined _:
                            case JSNull _:
                                radix = 10;
                                break;
                            case JSNumber jn:
                                radix = (int)jn.value;
                                break;
                            default:
                                double ra1 = NumberParser.ParseInt(a1.ToString().Trim(), 10, false);
                                if (!double.IsNaN(ra1))
                                {
                                    radix = (int)ra1;
                                    if (radix < 0 || radix == 1 || radix > 36)
                                        return nan;
                                }
                                break;
                        }
                    }
                    var d = NumberParser.ParseInt(text.Trim(), radix, false);
                    return new JSNumber(d);
                }
            }
            return nan;
        }

        public static JSString ToString(JSValue t, JSArray a)
        {
            var p = t;
            if (!(p is JSNumber n))
                throw JSContext.Current.TypeError($"Number.prototype.toExponential requires that 'this' be a Number");
            return new JSString(n.value.ToString());
        }

        public static JSString ToExponential(JSValue t, JSArray a)
        {
            var p = t;
            if (!(p is JSNumber n))
                throw JSContext.Current.TypeError($"Number.prototype.toExponential requires that 'this' be a Number");
            if (a._length > 0 && a[0] is JSNumber n1)
            {
                var v = n1.value;
                if (double.IsNaN(v) || v > 100 || v < 1)
                    throw JSContext.Current.RangeError("toExponential() digitis argument must be between 0 and 100");
                var fx = $"#.{new string('#',(int)v)}e+0";
                return new JSString(n.value.ToString(fx));
            }
            return new JSString(n.value.ToString("#.#################e+0"));
        }

        public static JSString ToFixed(JSValue t, JSArray a)
        {
            var p = t;
            if (!(p is JSNumber n))
                throw JSContext.Current.TypeError($"Number.prototype.toFixed requires that 'this' be a Number");
            if (a._length > 0 && a[0] is JSNumber n1)
            {
                if (double.IsNaN(n1.value) || n1.value > 100 || n1.value < 1)
                    throw JSContext.Current.RangeError("toFixed() digitis argument must be between 0 and 100");
                var i = (int)n1.value;
                return new JSString(n.value.ToString($"F{i}"));
            }
            return new JSString(n.value.ToString("F0"));
        }

        public static JSString ToPrecision(JSValue t, JSArray a)
        {
            var p = t;
            if (!(p is JSNumber n))
                throw JSContext.Current.TypeError($"Number.prototype.toFixed requires that 'this' be a Number");
            if (a._length > 0 && a[0] is JSNumber n1)
            {
                if (double.IsNaN(n1.value) || n1.value > 100 || n1.value < 1)
                    throw JSContext.Current.RangeError("toPrecision() digitis argument must be between 0 and 100");
                var i = (int)n1.value;
                var d = n.value;
                var prefix = 'G';
                if (d < 1)
                {
                    prefix = 'F';
                    // increase i for below 1
                    while (d < 1)
                    {
                        d = d * 10;
                        i++;
                    }
                    i--;
                }
                var txt = n.value.ToString($"{prefix}{i}")
                    .ToLower()
                    .Replace("e+0","0e+");
                return new JSString(txt);
            }
            return new JSString(n.value.ToString("G2"));
        }

        public static JSString ToLocaleString(JSValue t, JSArray a)
        {
            var p = t;
            if (!(p is JSNumber n))
                throw JSContext.Current.TypeError($"Number.prototype.toFixed requires that 'this' be a Number");
            if(a._length > 0)
            {
                var p1 = a[0];
                switch (p1)
                {
                    case JSNull _:
                    case JSUndefined _:
                        throw JSContext.Current.TypeError($"Cannot convert undefined or null to object");
                }
                var text = p1.ToString();
                var ci = CultureInfo.GetCultureInfo(text);
                if (ci == null)
                    throw JSContext.Current.RangeError("Incorrect locale information provided");
                return new JSString( n.value.ToString(ci.NumberFormat));
            }
            return new JSString(n.value.ToString("N2"));
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

            prototype.DefineProperty("toFixed", JSProperty.Function(ToFixed));

            prototype.DefineProperty("toPrecision", JSProperty.Function(ToPrecision));

            prototype.DefineProperty("toLocaleString", JSProperty.Function(ToLocaleString));

            prototype.DefineProperty("toString", JSProperty.Function(ToString));

            return r;
        }
    }
}
