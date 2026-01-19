
using System;
using System.Numerics;

namespace YantraJS.Core.BigInt;

internal partial class JSBigInt
{
    class AsUintNMethod : JSFunction
    {
        public AsUintNMethod(JSObject prototype) : base("asUIntN")
        {

        }

        public sealed override JSValue InvokeFunction(in Arguments a)
        {
            var bits = JSValueToClrConverter.ToLong(a[0], "bits");
            var bigint = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.BigInt.JSBigInt>(a[1], "bigint");
            if (bits < 0 || bits > 9007199254740991)
            {
                throw JSContext.Current.NewRangeError("Invalid range for bits");
            }
            var n = bigint.value;
            if (n.Sign == BigInteger.MinusOne.Sign)
            {
                n = -n;
            }
            var buffer = n.ToByteArray();
            if (buffer.Length * 8 < bits)
            {
                return bigint;
            }

            var reminderBits = (long)bits % (long)8;

            var length = (int)((long)bits / (long)8);
            if (reminderBits > 0)
            {
                length++;
            }

            // extra pad will result in a UInt
            var copy = new byte[length + 1];
            Buffer.BlockCopy(buffer, 0, copy, 0, length);

            if (reminderBits > 0)
            {
                ref byte last = ref copy[length - 1];
                byte start = 1;
                reminderBits--;
                while (reminderBits > 0)
                {
                    start <<= 1;
                    start |= 1;
                    reminderBits--;
                }
                last &= start;
            }

            var r = new BigInteger(copy);
            return new JSBigInt(r);
        }
    }
}
