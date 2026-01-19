
using System;
using System.Numerics;

namespace YantraJS.Core.BigInt;

internal partial class JSBigInt
{
    class AsIntNMethod : JSFunction
    {
        public AsIntNMethod(JSObject prototype) : base("asIntN")
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

            var copy = new byte[length];
            Buffer.BlockCopy(buffer, 0, copy, 0, length);

            if (reminderBits > 0)
            {
                // here we need to pad leftmost bits as 1s
                // as BigInteger uses bytes and only if the
                // eighth bit is 1, it will consider it as a
                // negative integer

                // so we need to create mask to first remove
                // bits as byte contains eight bits

                // then check the most significant digit
                // if it is negative, then we need to pad
                // 1s before it

                ref byte last = ref copy[copy.Length - 1];

                byte padMask = 0xFF;

                byte mask = 1;
                byte start = 1;
                reminderBits--;
                while (reminderBits > 0)
                {
                    padMask &= (byte)~start;
                    start <<= 1;
                    start |= 1;
                    mask <<= 1;
                    reminderBits--;
                }
                last &= start;
                var lastValue = last;

                if ((mask & lastValue) > 0)
                {
                    last |= padMask;
                }
            }

            var r = new BigInteger(copy);
            return new JSBigInt(r);
        }
    }
}
