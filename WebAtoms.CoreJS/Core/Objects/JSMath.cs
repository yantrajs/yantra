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
        /// https://github.com/paulbartrum/jurassic/blob/0522bcb42b29f87bdf65ae74b9a450179c1d168d/Jurassic/Library/MathObject.cs#L462
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
        /// https://github.com/paulbartrum/jurassic/blob/0522bcb42b29f87bdf65ae74b9a450179c1d168d/Jurassic/Library/MathObject.cs#L462
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

        /// <summary>
        /// https://github.com/paulbartrum/jurassic/blob/0522bcb42b29f87bdf65ae74b9a450179c1d168d/Jurassic/Library/MathObject.cs#L144
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>

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

        /// <summary>
        /// https://github.com/paulbartrum/jurassic/blob/0522bcb42b29f87bdf65ae74b9a450179c1d168d/Jurassic/Library/MathObject.cs#L475
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Static("atanh")]
        public static JSValue Atanh(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = new JSNumber(Math.Log((1.0 + d) / (1.0 - d)) / 2.0);
            return r;
        }

        /// <summary>
        /// https://github.com/paulbartrum/jurassic/blob/0522bcb42b29f87bdf65ae74b9a450179c1d168d/Jurassic/Library/MathObject.cs#L475
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Static("cbrt")]
        public static JSValue Cbrt(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = Math.Pow(Math.Abs(d), 1.0 / 3.0);
            return new JSNumber (d < 0 ? -r : r);
        }

        [Static("ceil")]
        public static JSValue Ceil(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = new JSNumber(Math.Ceiling(d));
            return r;
        }

        private static readonly int[] clz32Table = new int[] {
            32, 31,  0, 16,  0, 30,  3,  0, 15,  0,  0,  0, 29, 10,  2,  0,
             0,  0, 12, 14, 21,  0, 19,  0,  0, 28,  0, 25,  0,  9,  1,  0,
            17,  0,  4,  0,  0,  0, 11,  0, 13, 22, 20,  0, 26,  0,  0, 18,
             5,  0,  0, 23,  0, 27,  0,  6,  0, 24,  7,  0,  8,  0,  0,  0
        };


        /// <summary>
        /// we have Int value, so we might want to replace DoubleValue with Intvalue, 
        /// But since the implementation is not complete, we have continued with Doublevalue
        /// https://github.com/paulbartrum/jurassic/blob/0522bcb42b29f87bdf65ae74b9a450179c1d168d/Jurassic/Library/MathObject.cs#L475
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Static("clz32")]
        public static JSValue Clz32(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var x = ((uint)d) >> 0;
            x = x | (x >> 1);       // Propagate leftmost
            x = x | (x >> 2);       // 1-bit to the right.
            x = x | (x >> 4);
            x = x | (x >> 8);
            x = x | (x >> 16);
            x = x * 0x06EB14F9;     // Multiplier is 7*255**3.
            var r= clz32Table[x >> 26];
            return new JSNumber(r);
        }

        [Static("cos")]
        public static JSValue Cos(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = new JSNumber(Math.Cos(d));
            return r;
        }

        [Static("cosh")]
        public static JSValue Cosh(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = new JSNumber(Math.Cosh(d));
            return r;
        }

        [Static("exp")]
        public static JSValue Exp(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = new JSNumber(Math.Exp(d));
            return r;
        }

        /// <summary>
        /// https://github.com/paulbartrum/jurassic/blob/0522bcb42b29f87bdf65ae74b9a450179c1d168d/Jurassic/Library/MathObject.cs#L397
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Static("expm1")]
        public static JSValue Expm1(in Arguments args)
        {
            var first = args.Get1();
            double r;
            var d = first.DoubleValue;
            if (Math.Abs(d) < 0.01)
            {
                // For small numbers, use a taylor series approximation.
                r = d * (1.0 + d * (1.0 / 2.0 + d * (1.0 / 6.0 + d *
                    (1.0 / 24.0 + d * (1.0 / 120.0 + d * (1.0 / 720.0 + d * (1.0 / 5040.0)))))));
                return new JSNumber(r) ;
            }
            // Otherwise just use the normal exp function.
            r = Math.Exp(d) - 1.0;
            return new JSNumber(r);
            
        }

        /// <summary>
        /// https://github.com/paulbartrum/jurassic/blob/0522bcb42b29f87bdf65ae74b9a450179c1d168d/Jurassic/Library/MathObject.cs#L569
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Static("fround")]
        public static JSValue Fround(in Arguments args)
        {
            var first = args.Get1();
            var d = first.DoubleValue;
            var r = (double)(float)d;
            return new JSNumber(r);
            
        }

        /// <summary>
        /// https://github.com/paulbartrum/jurassic/blob/0522bcb42b29f87bdf65ae74b9a450179c1d168d/Jurassic/Library/MathObject.cs#L489
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Static("hypot")]
        public static JSValue Hypot(in Arguments args)
        {
            int length = args.Length;
            if (length == 0)
                return JSNumber.Zero;
            if (length == 1) {
                return new JSNumber(Math.Abs(args.Get1().DoubleValue));
            }
            var (first, second) = args.Get2();
            double d1 = first.DoubleValue;
            double d2 = second.DoubleValue;
            
            if (length == 2)
                return new JSNumber(Hypot(d1, d2));

            double result = Hypot(d1, d2);
            for (int i = 2; i < length; i++) {
                double val = args.GetAt(i).DoubleValue;

                result = Hypot(result, val);
            }
            return new JSNumber(result);

        }

        /// <summary>
        /// https://github.com/paulbartrum/jurassic/blob/0522bcb42b29f87bdf65ae74b9a450179c1d168d/Jurassic/Library/MathObject.cs#L511
        /// </summary>
        /// <param name="number1"></param>
        /// <param name="number2"></param>
        /// <returns></returns>
        public static double Hypot(double number1, double number2) {
            double abs1 = Math.Abs(number1);
            double abs2 = Math.Abs(number2);
            double min = Math.Min(abs1, abs2);
            double max = Math.Max(abs1, abs2);
            double u = min / max;
            if (min == 0)
                return max;
            return max * Math.Sqrt(1 + u * u);

        }


    }
}
