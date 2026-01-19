using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace YantraJS.Core.BigInt;

partial class JSBigInt
{

    internal partial class Constructor : JSFunction
    {
        public Constructor(JSObject prototype) : base("constructor", 1, true, prototype)
        {

        }

        public sealed override JSValue InvokeFunction(in Arguments a)
        {
            var f = a[0];
            switch (f)
            {
                case JSNumber number:
                    return new JSBigInt((BigInteger)number.value);
                case JSBigInt bigint:
                    return bigint;
            }
            var text = f.ToString();
            text = text.TrimEnd('n').Replace("_", "");
            if (!BigInteger.TryParse(text, out var v))
            {
                throw JSContext.Current.NewTypeError($"{f} is not a valid big integer");
            }
            return new JSBigInt(v);
        }

        public sealed override JSValue CreateInstance(in Arguments a)
        {
            return this.InvokeFunction(a);
        }
    }

}