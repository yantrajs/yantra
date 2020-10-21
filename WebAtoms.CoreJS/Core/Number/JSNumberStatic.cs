using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core.Runtime
{
    public static class JSNumberStatic

    {


        [Static("isFinite")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue IsFinite(in Arguments a)
        {
            if (a.Get1() is JSNumber n)
            {
                if (n.value != double.NaN && n.value > Double.NegativeInfinity && n.value < double.PositiveInfinity)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Static("isInteger")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue IsInteger(in Arguments a)
        {
            if (a.Get1() is JSNumber n)
            {
                var v = n.value;
                if (((int)v) == v)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Static("isNaN")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue IsNaN(in Arguments a)
        {
            if (a.Get1() is JSNumber n)
            {
                if (double.IsNaN(n.value))
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Static("isSafeInteger")]
        public static JSValue IsSafeInteger(in Arguments a)
        {
            if (a.Get1() is JSNumber n)
            {
                var v = n.value;
                if (v >= JSNumber.MinSafeInteger && v <= JSNumber.MaxSafeInteger)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Static("parseFloat")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue ParseFloat(in Arguments a)
        {
            var nan = JSNumber.NaN;
            if (a.Length > 0)
            {
                var p = a.Get1();
                if (p.IsNumber)
                    return p;
                if (p.IsNull || p.IsUndefined)
                    return nan;
                var text = p.JSTrim();
                if (text.Length > 0)
                {
                    int start = 0;
                    char ch;
                    bool hasDot = false;
                    bool hasE = false;
                    do
                    {
                        ch = text[start];
                        if (char.IsDigit(ch))
                        {
                            start++;
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


        [Static("parseInt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue ParseInt(in Arguments a)
        {
            var nan = JSNumber.NaN;
            if (a.Length > 0)
            {
                var p = a.Get1();
                if (p.IsNumber)
                    return p;
                if (p.IsNull || p.IsUndefined)
                    return nan;
                var text = p.JSTrim();
                if (text.Length > 0)
                {
                    var radix = 10;
                    if (a.Length > 2)
                    {
                        var (_, a1) = a.Get2();
                        if (a1.IsNull || a1.IsUndefined)
                        {
                            radix = 10;
                        }
                        else
                        {
                            var n = a1.DoubleValue;
                            if (!double.IsNaN(n))
                            {
                                radix = (int)n;
                                if (radix < 0 || radix == 1 || radix > 36)
                                    return nan;
                            }
                        }
                    }
                    var d = NumberParser.ParseInt(text.Trim(), radix, false);
                    return new JSNumber(d);
                }
            }
            return nan;
        }
    }
}
