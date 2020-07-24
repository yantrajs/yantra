using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace WebAtoms.CoreJS.Utils
{
    class NumberParser
    {
        /// <summary>
        /// Converts a string to an integer (used by parseInt).
        /// </summary>
        /// <param name="input"> The input text to parse. </param>
        /// <param name="radix"> The numeric base to use for parsing.  Pass zero to use base 10
        /// except when the input string starts with '0' in which case base 16 or base 8 are used
        /// instead. </param>
        /// <param name="allowOctal"> <c>true</c> if numbers with a leading zero should be parsed
        /// as octal numbers. </param>
        /// <returns> The result of parsing the string as a integer. </returns>
        internal static double ParseInt(string input, int radix, bool allowOctal)
        {
            var reader = new System.IO.StringReader(input);
            int digitCount = 0;

            // Skip whitespace and line terminators.
            while (IsWhiteSpaceOrLineTerminator(reader.Peek()))
                reader.Read();

            // Determine the sign.
            double sign = 1;
            if (reader.Peek() == '+')
            {
                reader.Read();
            }
            else if (reader.Peek() == '-')
            {
                sign = -1;
                reader.Read();
            }

            // Hex prefix should be stripped if the radix is 0, undefined or 16.
            bool stripPrefix = radix == 0 || radix == 16;

            // Default radix is 10.
            if (radix == 0)
                radix = 10;

            // Skip past the prefix, if there is one.
            if (stripPrefix == true)
            {
                if (reader.Peek() == '0')
                {
                    reader.Read();
                    digitCount = 1;     // Note: required for parsing "0z11" correctly (when radix = 0).

                    int c = reader.Peek();
                    if (c == 'x' || c == 'X')
                    {
                        // Hex number.
                        reader.Read();
                        radix = 16;
                    }

                    if (c >= '0' && c <= '9' && allowOctal == true)
                    {
                        // Octal number.
                        radix = 8;
                    }
                }
            }

            // Calculate the maximum number of digits before arbitrary precision arithmetic is
            // required.
            int maxDigits = (int)Math.Floor(53 / Math.Log(radix, 2));

            // Read numeric digits 0-9, a-z or A-Z.
            double result = 0;
            var bigResult = BigInteger.Zero;
            while (true)
            {
                int numericValue = -1;
                int c = reader.Read();
                if (c >= '0' && c <= '9')
                    numericValue = c - '0';
                if (c >= 'a' && c <= 'z')
                    numericValue = c - 'a' + 10;
                if (c >= 'A' && c <= 'Z')
                    numericValue = c - 'A' + 10;
                if (numericValue == -1 || numericValue >= radix)
                    break;
                if (digitCount == maxDigits)
                    bigResult = new BigInteger(result);
                result = result * radix + numericValue;
                if (digitCount >= maxDigits)
                    bigResult += radix * numericValue;
                digitCount++;
            }

            // If the input is empty, then return NaN.
            if (digitCount == 0)
                return double.NaN;

            // Numbers with lots of digits require the use of arbitrary precision arithmetic to
            // determine the correct answer.
            if (digitCount > maxDigits)
                return RefineEstimate(result, 0, bigResult) * sign;

            return result * sign;
        }

        /// <summary>
        /// Parses a hexidecimal number and returns the corresponding double-precision value.
        /// </summary>
        /// <param name="reader"> The reader to read characters from. </param>
        /// <returns> The numeric value, or <c>NaN</c> if the number is invalid. </returns>
        internal static double ParseHex(TextReader reader)
        {
            double result = 0;
            int digitsRead = 0;

            // Read numeric digits 0-9, a-f or A-F.
            while (true)
            {
                int c = reader.Peek();
                if (c >= '0' && c <= '9')
                    result = result * 16 + c - '0';
                else if (c >= 'a' && c <= 'f')
                    result = result * 16 + c - 'a' + 10;
                else if (c >= 'A' && c <= 'F')
                    result = result * 16 + c - 'A' + 10;
                else
                    break;
                digitsRead++;
                reader.Read();
            }
            if (digitsRead == 0)
                return double.NaN;
            return result;
        }

        /// <summary>
        /// Parses a octal number and returns the corresponding double-precision value.
        /// </summary>
        /// <param name="reader"> The reader to read characters from. </param>
        /// <returns> The numeric value, or <c>NaN</c> if the number is invalid. </returns>
        internal static double ParseOctal(TextReader reader)
        {
            double result = 0;

            // Read numeric digits 0-7.
            while (true)
            {
                int c = reader.Peek();
                if (c >= '0' && c <= '7')
                    result = result * 8 + c - '0';
                else if (c == '8' || c == '9')
                    return double.NaN;
                else
                    break;
                reader.Read();
            }
            return result;
        }

        /// <summary>
        /// Parses a binary number and returns the corresponding double-precision value.
        /// </summary>
        /// <param name="reader"> The reader to read characters from. </param>
        /// <returns> The numeric value, or <c>NaN</c> if the number is invalid. </returns>
        internal static double ParseBinary(TextReader reader)
        {
            double result = 0;

            // Read numeric digits 0-1.
            while (true)
            {
                int c = reader.Peek();
                if (c == '0')
                    result = result * 2;
                else if (c == '1')
                    result = result * 2 + 1;
                else if (c >= '2' && c <= '9')
                    return double.NaN;
                else
                    break;
                reader.Read();
            }
            return result;
        }

        /// <summary>
        /// Determines if the given character is whitespace or a line terminator.
        /// </summary>
        /// <param name="c"> The unicode code point for the character. </param>
        /// <returns> <c>true</c> if the character is whitespace or a line terminator; <c>false</c>
        /// otherwise. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsWhiteSpaceOrLineTerminator(int c)
        {
            return c == 9 || c == 0x0b || c == 0x0c || c == ' ' || c == 0xa0 || c == 0xfeff ||
                c == 0x1680 || c == 0x180e || (c >= 0x2000 && c <= 0x200a) || c == 0x202f || c == 0x205f || c == 0x3000 ||
                c == 0x0a || c == 0x0d || c == 0x2028 || c == 0x2029;
        }

        /// <summary>
        /// Modifies the initial estimate until the closest double-precision number to the desired
        /// value is found.
        /// </summary>
        /// <param name="initialEstimate"> The initial estimate.  Assumed to be very close to the
        /// result. </param>
        /// <param name="base10Exponent"> The power-of-ten scale factor. </param>
        /// <param name="desiredValue"> The desired value, already scaled using the power-of-ten
        /// scale factor. </param>
        /// <returns> The closest double-precision number to the desired value.  If there are two
        /// such values, the one with the least significant bit set to zero is returned. </returns>
        private static double RefineEstimate(double initialEstimate, int base10Exponent, BigInteger desiredValue)
        {
            // Numbers with 16 digits or more are tricky because rounding error can cause the
            // result to be out by one or more ULPs (units in the last place).
            // The algorithm is as follows:
            // 1.  Use the initially calculated result as an estimate.
            // 2.  Create a second estimate by modifying the estimate by one ULP.
            // 3.  Calculate the actual value of both estimates to precision X (using arbitrary
            //     precision arithmetic).
            // 4.  If they are both above the desired value then decrease the estimates by 1
            //     ULP and goto step 3.
            // 5.  If they are both below the desired value then increase up the estimates by
            //     1 ULP and goto step 3.
            // 6.  One estimate must now be above the desired value and one below.
            // 7.  If one is estimate is clearly closer to the desired value than the other,
            //     then return that estimate.
            // 8.  Increase the precision by 32 bits.
            // 9.  If the precision is less than or equal to 160 bits goto step 3.
            // 10. Assume that the estimates are equally close to the desired value; return the
            //     value with the least significant bit equal to 0.
            int direction = double.IsPositiveInfinity(initialEstimate) ? -1 : 1;
            int precision = 32;

            // Calculate the candidate value by modifying the last bit.
            double result = initialEstimate;
            double result2 = AddUlps(result, direction);

            // Figure out our multiplier.  Either base10Exponent is positive, in which case we
            // multiply actual1 and actual2, or it's negative, in which case we multiply
            // desiredValue.
            BigInteger multiplier = BigInteger.One;
            if (base10Exponent < 0)
                multiplier = BigInteger.Pow(10, -base10Exponent);
            else if (base10Exponent > 0)
                desiredValue = BigInteger.Multiply(desiredValue, BigInteger.Pow(10, base10Exponent));

            while (precision <= 160)
            {
                // Scale the candidate values to a big integer.
                var actual1 = ScaleToInteger(result, multiplier, precision);
                var actual2 = ScaleToInteger(result2, multiplier, precision);

                // Calculate the differences between the candidate values and the desired value.
                var baseline = desiredValue << precision;
                var diff1 = BigInteger.Subtract(actual1, baseline);
                var diff2 = BigInteger.Subtract(actual2, baseline);

                if (diff1.Sign == direction && diff2.Sign == direction)
                {
                    // We're going the wrong way!
                    direction = -direction;
                    result2 = AddUlps(result, direction);
                }
                else if (diff1.Sign == -direction && diff2.Sign == -direction)
                {
                    // Going the right way, but need to go further.
                    result = result2;
                    result2 = AddUlps(result, direction);
                }
                else
                {
                    // Found two values that bracket the actual value.
                    // If one candidate value is closer to the actual value by at least 2 (one
                    // doesn't cut it because of the integer division) then use that value.
                    diff1 = BigInteger.Abs(diff1);
                    diff2 = BigInteger.Abs(diff2);
                    if (BigInteger.Compare(diff1, BigInteger.Subtract(diff2, BigInteger.One)) < 0)
                        return result;
                    if (BigInteger.Compare(diff2, BigInteger.Subtract(diff1, BigInteger.One)) < 0)
                        return result2;

                    // Not enough precision to determine the correct answer, or it's a halfway case.
                    // Increase the precision.
                    precision += 32;
                }

                // If result2 is NaN then we have gone too far.
                if (double.IsNaN(result2) == true)
                    return result;
            }

            // Even with heaps of precision there is no clear winner.
            // Assume this is a halfway case: choose the floating-point value with its least
            // significant bit equal to 0.
            return (BitConverter.DoubleToInt64Bits(result) & 1) == 0 ? result : result2;
        }

        /// <summary>
        /// Adds ULPs (units in the last place) to the given double-precision number.
        /// </summary>
        /// <param name="value"> The value to modify. </param>
        /// <param name="ulps"> The number of ULPs to add.  Can be negative. </param>
        /// <returns> The modified number. </returns>
        private static double AddUlps(double value, int ulps)
        {
            // Note: overflow or underflow in mantissa carries over to the exponent.
            // Overflow or underflow in exponent produces infinity or zero.
            return BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(value) + ulps);
        }

        /// <summary>
        /// Scales the given double-precision number by multiplying and then shifting it.
        /// </summary>
        /// <param name="value"> The value to scale. </param>
        /// <param name="multiplier"> The multiplier. </param>
        /// <param name="shift"> The power of two scale factor. </param>
        /// <returns> A BigInteger containing the result of multiplying <paramref name="value"/> by
        /// <paramref name="multiplier"/> and then shifting left by <paramref name="shift"/> bits. </returns>
        private static BigInteger ScaleToInteger(double value, BigInteger multiplier, int shift)
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

            var result = new BigInteger(mantissa);
            result = BigInteger.Multiply(result, multiplier);
            shift += base2Exponent;
            result = result << shift;
            return result;
        }

    }
}
