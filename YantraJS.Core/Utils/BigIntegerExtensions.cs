using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace YantraJS.Utils
{
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// Returns a new instance BigInteger structure from a 64-bit double precision floating
        /// point value.
        /// </summary>
        /// <param name="value"> A 64-bit double precision floating point value. </param>
        /// <returns> The corresponding BigInteger value. </returns>
        public static BigInteger FromDouble(this double value)
        {
            long bits = BitConverter.DoubleToInt64Bits(value);

            // Extract the base-2 exponent.
            var base2Exponent = (int)((bits & 0x7FF0000000000000) >> 52) - 1023;

            // Extract the mantissa.
            long mantissa = bits & 0xFFFFFFFFFFFFF;
            if (base2Exponent > -1023)
            {
                mantissa |= 0x10000000000000;
                base2Exponent -= 52;
            }
            else
            {
                // Denormals.
                base2Exponent -= 51;
            }

            // Extract the sign bit.
            if (bits < 0)
                mantissa = -mantissa;

            return (new BigInteger(mantissa)).LeftShift(base2Exponent);
        }

        /// <summary>
        /// Shifts a BigInteger value a specified number of bits to the left.
        /// </summary>
        /// <param name="value"> The value whose bits are to be shifted. </param>
        /// <param name="shift"> The number of bits to shift <paramref name="value"/> to the left.
        /// Can be negative to shift to the right. </param>
        /// <returns> A value that has been shifted to the left by the specified number of bits. </returns>
        public static BigInteger LeftShift(this BigInteger value, int shift)
        {
            return value << shift;
        }

    }
}
