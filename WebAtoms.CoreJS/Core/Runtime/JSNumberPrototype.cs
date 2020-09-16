using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace WebAtoms.CoreJS.Core.Runtime
{
    public static class JSNumberPrototype
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static JSNumber ToNumber(this JSValue target, [CallerMemberName] string name = null)
        {
            if (!(target.IsNumber))
                throw JSContext.Current.NewTypeError($"Number.prototype.{name} requires that 'this' be a Number");
            return (JSNumber)target;
        }

        [Prototype("toString")]

        public static JSString ToString(JSValue t, params JSValue[] a)
        {
            var n = t.ToNumber();
            return new JSString(n.value.ToString());
        }

        [Prototype("toExponential")]

        public static JSString ToExponential(JSValue t, params JSValue[] a)
        {
            var n = t.ToNumber();
            if (a[0] is JSNumber n1)
            {
                var v = n1.value;
                if (double.IsNaN(v) || v > 100 || v < 1)
                    throw JSContext.Current.NewRangeError("toExponential() digitis argument must be between 0 and 100");
                var fx = $"#.{new string('#', (int)v)}e+0";
                return new JSString(n.value.ToString(fx));
            }
            return new JSString(n.value.ToString("#.#################e+0"));
        }

        [Prototype("toFixed")]
        public static JSString ToFixed(JSValue t, params JSValue[] a)
        {
            var n = t.ToNumber();
            if (a[0] is JSNumber n1)
            {
                if (double.IsNaN(n1.value) || n1.value > 100 || n1.value < 1)
                    throw JSContext.Current.NewRangeError("toFixed() digitis argument must be between 0 and 100");
                var i = (int)n1.value;
                return new JSString(n.value.ToString($"F{i}"));
            }
            return new JSString(n.value.ToString("F0"));
        }

        [Prototype("toPrecision")]
        public static JSString ToPrecision(JSValue t, params JSValue[] a)
        {
            var n = t.ToNumber();
            if (a[0] is JSNumber n1)
            {
                if (double.IsNaN(n1.value) || n1.value > 100 || n1.value < 1)
                    throw JSContext.Current.NewRangeError("toPrecision() digitis argument must be between 0 and 100");
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
                    .Replace("e+0", "0e+");
                return new JSString(txt);
            }
            return new JSString(n.value.ToString("G2"));
        }

        [Prototype("toLocaleString")]
        public static JSString ToLocaleString(JSValue t, params JSValue[] a)
        {
            var n = t.ToNumber();
            if (a.Length > 0)
            {
                var p1 = a[0];
                switch (p1)
                {
                    case JSNull _:
                    case JSUndefined _:
                        throw JSContext.Current.NewTypeError($"Cannot convert undefined or null to object");
                }
                var text = p1.ToString();
                var ci = CultureInfo.GetCultureInfo(text);
                if (ci == null)
                    throw JSContext.Current.NewRangeError("Incorrect locale information provided");
                return new JSString(n.value.ToString(ci.NumberFormat));
            }
            return new JSString(n.value.ToString("N2"));
        }
    }
}
