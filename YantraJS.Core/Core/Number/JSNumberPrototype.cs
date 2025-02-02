using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Primitive;

namespace YantraJS.Core
{
    internal static class JSNumberExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSNumber ToNumber(this JSValue target, [CallerMemberName] string name = null)
        {
            if (!(target is JSNumber n))
            {
                if (target is JSPrimitiveObject primitiveObject)
                {
                    return primitiveObject.value.ToNumber();
                }
                throw JSContext.Current.NewTypeError($"Number.prototype.{name} requires that 'this' be a Number");
            }
            return n;
        }

    }

    partial class JSNumber
    {

        [JSExport(Length = 1, IsConstructor = true)]
        public static JSValue Constructor(in Arguments a)
        {
            if (JSContext.Current.CurrentNewTarget == null)
            {
                if(a.Length == 0)
                {
                    return JSNumber.Zero;
                }
                return new JSNumber(a[0].DoubleValue);
            }
            if (a.Length == 0)
                return new JSPrimitiveObject(JSNumber.Zero);
            return new JSPrimitiveObject(new JSNumber(a.Get1().DoubleValue));
        }


        [JSPrototypeMethod]
        [JSExport("clz")]
        public static JSValue Clz(in Arguments a)
        {
            var x = a.This.ToNumber().IntValue;

            // Propagate leftmost 1-bit to the right 
            x = x | (x >> 1);
            x = x | (x >> 2);
            x = x | (x >> 4);
            x = x | (x >> 8);
            x = x | (x >> 16);

            int i = sizeof(int) * 8 - CountOneBits((uint)x);
            return new JSNumber(i);
        }

        /// <summary>
        /// Counts the number of set bits in an integer.
        /// </summary>
        /// <param name="x"> The integer. </param>
        /// <returns> The number of set bits in the integer. </returns>
        private static int CountOneBits(uint x)
        {
            x -= ((x >> 1) & 0x55555555);
            x = (((x >> 2) & 0x33333333) + (x & 0x33333333));
            x = (((x >> 4) + x) & 0x0f0f0f0f);
            x += (x >> 8);
            x += (x >> 16);
            return (int)(x & 0x0000003f);
        }

        [JSPrototypeMethod]
        [JSExport("valueOf")]
        public static JSValue ValueOf(in Arguments a)
        {
            return a.This.ToNumber();
        }

        [JSPrototypeMethod]
        [JSExport("toString", Length =1)]

        public static JSString ToString(in Arguments a)
        {
            var n = a.This.ToNumber();
            string result;
            var value = n.value;
            var arg = a.Get1();
            int radix = 0;
            var culture = CultureInfo.GetCultureInfo("en-US");
            if (!arg.IsNullOrUndefined)
            {
                radix = arg.IntValue;
                if (radix < 2 || radix > 36)
                    throw JSContext.Current.NewRangeError("The radix must be between 2 and 36, inclusive.");

                // return new JSString(Convert.ToString((int)value, radix));
                result = DecimalToBase(value, radix);
                return new JSString(result);

            }
            if (double.IsPositiveInfinity(value))
                return JSConstants.Infinity;
            if (double.IsNegativeInfinity(value))
                return JSConstants.NegativeInfinity;

            if (value > 999999999999999.0)
                return new JSString(value.ToString("g21", culture));
            if (value > 5e-7 && value < 1)
                return new JSString(value.ToString("f6", culture)); //Assert.AreEqual("0.000005", Evaluate("5e-6.toString()"));
            var txt = value.ToString("g", culture);
            var eIndex = txt.IndexOf('e'); // remove extra zero, after e if any. 
            if (eIndex != -1)
            {
                if (txt[eIndex + 2] == '0')
                {
                    txt = txt.Remove(eIndex + 2, 1);
                }
            }
            
            return new JSString(txt);

        }

        [JSPrototypeMethod]
        [JSExport("toExponential", Length = 1)]

        public static JSString ToExponential(in Arguments a)
        {
            var n = a.This.ToNumber();
            var nv = n.value;
            if (double.IsPositiveInfinity(nv))
                return JSConstants.Infinity;
            if (double.IsNegativeInfinity(nv))
                return JSConstants.NegativeInfinity;
            if (a.Length > 0)
            {
                if (a.Get1() is JSNumber n1)
                {

                    var v = n1.value;

                    if (double.IsNaN(v) || v > 20 || v < 0)
                        throw JSContext.Current.NewRangeError("toExponential() digitis argument must be between 0 and 100");
                    var m = (int)v;
                    if (m == 0)
                    {
                        // round..
                        return new JSString(nv.ToString("0e+0"));
                    }
                    var fx = $"#.{new string('0', m)}{new string('#', m != 0 ? 0 : 16 - m)}e+0";
                    return new JSString(nv.ToString(fx));
                }
            }

            var text = n.value.ToString("#.################e+0");
            //if (text.Length > 15) {
            //    return new JSString(n.value.ToString("r"));
            //}
            return new JSString(text);
            // return new JSString(n.value.ToString("g17"));
            // return new JSString(n.value.ToString());
        }

        [JSPrototypeMethod]
        [JSExport("toFixed", Length = 1)]
        public static JSString ToFixed(in Arguments a)
        {
            var n = a.This.ToNumber();
            var nv = n.value;
            if (double.IsPositiveInfinity(nv))
                return JSConstants.Infinity;
            if (double.IsNegativeInfinity(nv))
                return JSConstants.NegativeInfinity;
            if (a.Get1() is JSNumber n1)
            {
                if (double.IsNaN(n1.value) || n1.value > 20 || n1.value < 0)
                    throw JSContext.Current.NewRangeError("toFixed() digitis argument must be between 0 and 100");
                var i = (int)n1.value;
                if (nv > 999999999999999.0 && i <= 15)
                    return new JSString(nv.ToString("g21"));
                return new JSString(nv.ToString($"F{i}"));
            }
            if (nv > 999999999999999.0)
                return new JSString(nv.ToString("g21"));
            return new JSString(nv.ToString("F0"));
        }

        [JSPrototypeMethod]
        [JSExport("toPrecision", Length = 1)]
        public static JSString ToPrecision(in Arguments a)
        {
            var n = a.This.ToNumber();

            if (double.IsPositiveInfinity(n.value))
                return JSConstants.Infinity;
            if (double.IsNegativeInfinity(n.value))
                return JSConstants.NegativeInfinity;

            if (a.Get1() is JSNumber n1)
            {
                if (double.IsNaN(n1.value) || n1.value > 21 || n1.value < 1)
                    throw JSContext.Current.NewRangeError("toPrecision() digits argument must be between 0 and 100");
                var i = (int)n1.value;
                var originalPrecision = i;
                var d = n.value;
                var prefix = 'g';
                var iteration = 0;
                if (d < 1)
                {
                    prefix = 'f';
                    // switch to f when number is less than 1
                    // because precision is measured from the first non zero
                    // digit position
                    // Assert.AreEqual("0.0000012", Evaluate("0.00000123.toPrecision(2)"));
                    while (d < 1)
                    {
                        d = d * 10;
                        i++;
                        iteration++;
                        if (iteration > 6)
                        {
                            // do this only 6 times
                            // or switch back to g
                            // Assert.AreEqual("1.2e-7", Evaluate("0.000000123.toPrecision(2)"));
                            prefix = 'g';
                            i = originalPrecision + 1;
                            break;
                        }
                    }
                    i--;
                }
                string txt;
                txt = n.value.ToString($"{prefix}{i}");

                // add trailing zeros after .

                var eIndex = txt.IndexOf('e');
                if (eIndex != -1)
                {
                    if (txt[eIndex + 2] == '0')
                    {
                        txt = txt.Substring(0, eIndex + 2) + txt.Substring(eIndex + 3);
                    }
                    var totalDigits = eIndex;
                    var hasDot = txt.IndexOf('.');
                    if (hasDot != -1)
                    {
                        totalDigits--;
                    }
                    var diff = originalPrecision - totalDigits;
                    if (diff > 0)
                    {
                        if (hasDot == -1)
                        {
                            txt = txt.Insert(eIndex, ".");
                            eIndex++;
                        }
                        txt = txt.Insert(eIndex, new string('0', diff));
                    }
                }
                else
                {
                    var totalDigits = txt.Length;
                    var dotIndex = txt.IndexOf('.');
                    if (dotIndex != -1)
                    {
                        totalDigits--;
                    }
                    if (totalDigits < originalPrecision)
                    {
                        if (dotIndex == -1)
                            txt += ".";
                        var diff = originalPrecision - totalDigits;
                        txt += new string('0', diff);
                    }
                }
                //var result = string.Format("{0:0.00}", txt);
                return new JSString(txt);
            }
            return new JSString(n.value.ToString());
        }

        [JSPrototypeMethod]
        [JSExport("toLocaleString")]
        public static JSString ToLocaleString(in Arguments a)
        {
            var n = a.This.ToNumber();
            var (locale, format) = a.Get2();
            var formatting = "g";

            if (!locale.IsNullOrUndefined)
            {
                string number;
                var culture = CultureInfo.GetCultureInfo(locale.ToString());
                if (format.IsNullOrUndefined)
                {
                    number = n.value.ToString(formatting, culture);
                }
                else
                {
                    if (format.IsString)
                    {
                        number = n.value.ToString(format.ToString(), culture);
                    }
                    else
                    {
                        throw JSContext.Current.NewTypeError("Options not supported, use .Net String Formats");
                    }
                }
                return new JSString(number);
            }


            return new JSString(n.value.ToString(formatting, System.Globalization.CultureInfo.CurrentCulture));
        }

        public static string DecimalToBase(double number, int radix)
        {
            if (number == 0.0)
                return "0";
            if (double.IsPositiveInfinity(number))
                return "Infinity";
            if (double.IsNegativeInfinity(number))
                return "-Infinity";
            if (double.IsNaN(number))
                return "NaN";
            var isNegative = number < 0.0;
            number = Math.Abs(number);
            var digits = Math.Floor(number);
            var digitsTxt = DecimalToArbitrarySystem((long)digits, radix);
            if (digits == number)
                return digitsTxt;
            var fraction = number % digits;
            for (int i = 0; i < 15; i++)
            {
                fraction = fraction * 10;
                if (Math.Floor(fraction) == fraction)
                    break;
            }
            var fractionText = DecimalToArbitrarySystem((long)fraction, radix);
            return $"{(isNegative ? "-" : " ")}{digitsTxt}.{fractionText}";
        }


        /// <summary>
        /// https://stackoverflow.com/questions/923771/quickest-way-to-convert-a-base-10-number-to-any-base-in-net
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <returns></returns>
        public static string DecimalToArbitrarySystem(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789abcdefghijklmnopqrstuvwxyz";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new string(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }


    }
}
