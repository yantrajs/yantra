using System;
using System.Collections.Generic;
using System.Text;

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

        [Static("abs")]
        public static JSValue Abs(JSValue t, JSValue[] args)
        {
            return new JSNumber(Math.Abs(t.DoubleValue));
        }
             

        [Static("random")]
        public static JSValue Random(JSValue t, JSValue[] args)
        {
            var r = randomGenertor ?? (randomGenertor = new Random());
            return new JSNumber(r.NextDouble());
        }

        [Static("round")]
        public static JSValue Round(JSValue t, JSValue[] args)
        {
            var number = args[0].DoubleValue;
            if (number > 0.0)
                return new JSNumber(Math.Floor(number + 0.5));
            if (number >= -0.5)
            {
                // BitConverter is used to distinguish positive and negative zero.
                if (BitConverter.DoubleToInt64Bits(number) == 0L)
                    return JSContext.Current.Zero;
                return new JSNumber(-0.0D);
            }
            return new JSNumber( Math.Floor(number + 0.5));
        }

    }
}
