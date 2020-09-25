using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core.Objects
{
    public class JSMath: JSObject
    {
        static System.Random randomGenertor;

        [Static(nameof(E))]
        public static double E = Math.E;

        [Static(nameof(LN10))]
        public static double LN10 = Math.Log(10);

        [Static(nameof(LN2))]
        public static double LN2 = Math.Log(2);

        [Static(nameof(LOG10E))]
        public static double LOG10E = Math.Log10(E);

        [Static(nameof(LOG2E))]
        public static double LOG2E = Math.Log(E);

        [Static(nameof(PI))]
        public static double PI = Math.PI;

        [Static(nameof(SQRT1_2))]
        public static double SQRT1_2 = Math.Sqrt(0.5);

        [Static(nameof(SQRT2))]
        public static double SQRT2 = Math.Sqrt(2);

        [Static("random")]
        public static JSValue Random(in Arguments a)
        {
            var r = randomGenertor ?? (randomGenertor = new Random());
            return new JSNumber(r.NextDouble());
        }

        [Static("round")]
        public static JSValue Round(in Arguments args)
        {
            var first = args.Get1();
            if (first.IsUndefined)
                return JSNumber.NaN;
            if (first.IsNull)
                return JSNumber.Zero;
            var number = first.DoubleValue;
            if (number > 0.0)
                return new JSNumber(Math.Floor(number + 0.5));
            if (number >= -0.5)
            {
                // BitConverter is used to distinguish positive and negative zero.
                if (BitConverter.DoubleToInt64Bits(number) == 0L)
                    return JSNumber.Zero;
                return new JSNumber(-0.0D);
            }
            return new JSNumber( Math.Floor(number + 0.5));
        }

        /// <summary>
        /// We do not want to recreate new objects for standard known constants. 
        /// Hence, we ned to check and return already existing constants.
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Static("floor")]
        public static JSValue Floor(in Arguments args) {
            var first = args.Get1();
            var d = first.DoubleValue;
            //if (double.IsNaN(d))
            //    return JSNumber.NaN;
            //if (double.IsPositiveInfinity(d))
            //    return JSNumber.PositiveInfinity;
            //if (double.IsNegativeInfinity(d))
            //    return JSNumber.NegativeInfinity;
            var r = new JSNumber(Math.Floor(d));
            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Static("acos")]
        public static JSValue Acos(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            //if (double.IsNaN(d))
            //    return JSNumber.NaN;
            //if (double.IsPositiveInfinity(d))
            //    return JSNumber.PositiveInfinity;
            //if (double.IsNegativeInfinity(d))
            //    return JSNumber.NegativeInfinity;
            var r = new JSNumber(Math.Acos(d));
            return r;
        }

        [Static("abs")]
        public static JSValue Abs(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            //if (double.IsNaN(d))
            //    return JSNumber.NaN;
            //if (double.IsPositiveInfinity(d))
            //    return JSNumber.PositiveInfinity;
            //if (double.IsNegativeInfinity(d))
            //    return JSNumber.NegativeInfinity;
            var r = new JSNumber(Math.Abs(d));
            return r;
        }

        /// <summary>
        /// Ref. Jurrasic - MathObject.cs
        /// public static double Acosh(double number)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>

        [Static("acosh")]
        public static JSValue Acosh(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = new JSNumber(Math.Log(d + Math.Sqrt((d * d) - 1.0)));
            return r;
        }

        [Static("asin")]
        public static JSValue Asin(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = new JSNumber(Math.Asin(d));
            return r;
        }

        /// <summary>
        /// Ref. Jurrasic - MathObject.cs
        /// public static double Asinh(double number)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Static("asinh")]
        public static JSValue Asinh(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = new JSNumber(Math.Log(d + Math.Sqrt(d * d + 1.0)));
            return r;
        }

        [Static("atan")]
        public static JSValue Atan(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = new JSNumber(Math.Atan(d));
            return r;
        }

        [Static("atan2")]
        public static JSValue Atan2(in Arguments args)
        {
            var (first, second) = args.Get2();
            var d1 = first.DoubleValue;
            var d2 = second.DoubleValue;

            if (double.IsInfinity(d1) || double.IsInfinity(d2))
            {
                if (double.IsPositiveInfinity(d1) && double.IsPositiveInfinity(d2))
                    return new JSNumber(Math.PI / 4.0);
                if (double.IsPositiveInfinity(d1) && double.IsNegativeInfinity(d2))
                    return new JSNumber(3.0 * Math.PI / 4.0);
                if (double.IsNegativeInfinity(d1) && double.IsPositiveInfinity(d2))
                    return new JSNumber(-Math.PI / 4.0);
                if (double.IsNegativeInfinity(d1) && double.IsNegativeInfinity(d2))
                    return new JSNumber(-3.0 * Math.PI / 4.0);
            }
            var r = new JSNumber(Math.Atan2(d1,d2));
            return r;
        }
    }
}
