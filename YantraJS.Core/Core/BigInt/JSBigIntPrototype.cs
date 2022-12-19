using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace YantraJS.Core.BigInt
{
    internal static class JSBigIntPrototype
    {

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            var f = a.Get1();
            switch(f)
            {
                case JSNumber number:
                    return new JSBigInt((long)number.value);
                case JSBigInt bigint:
                    return bigint;
            }
            return new JSBigInt(f.ToString());
        }

        [Prototype("toString")]
        public static JSValue ToString(in Arguments a)
        {
            if (!(a.Get1() is JSBigInt bigint))
                throw JSContext.Current.NewTypeError($"Not a BigInt");
            return new JSString(bigint.value.ToString());
        }

        [Prototype("toLocaleString")]
        public static JSValue ToLocaleString(in Arguments a)
        {
            if (!(a.Get1() is JSBigInt bigint))
                throw JSContext.Current.NewTypeError($"Not a BigInt");
            return new JSString(bigint.value.ToString(CultureInfo.CurrentCulture));
        }


        [Prototype("valueOf")]
        public static JSValue ValueOf(in Arguments a)
        {
            if (!(a.Get1() is JSBigInt bigint))
                throw JSContext.Current.NewTypeError($"Not a BigInt");
            return bigint;
        }

    }
}
