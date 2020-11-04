using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Utils
{
    internal static class UriHelper
    {

        /// <summary>
        /// Creates a 128 entry lookup table for the characters in the given string.
        /// </summary>
        /// <param name="characters"> The characters to include in the set. </param>
        /// <returns> An array containing <c>true</c> for each character in the set. </returns>
        private static bool[] CreateCharacterSetLookupTable(string characters)
        {
            var result = new bool[128];
            for (int i = 0; i < characters.Length; i++)
            {
                char c = characters[i];
                if (c >= 128)
                    throw new ArgumentException(nameof(characters));
                result[c] = true;
            }
            return result;
        }

        private static bool[] decodeURIComponentReservedSet;


        /// <summary>
        /// Ref: https://github.com/paulbartrum/jurassic/blob/1e9b24b4926740aa2c1b0df8169398b3d340f681/Jurassic/Library/GlobalObject.cs#L511
        /// 
        /// Parses a hexidecimal number from within a string.
        /// </summary>
        /// <param name="input"> The string containing the hexidecimal number. </param>
        /// <param name="start"> The start index of the hexidecimal number. </param>
        /// <param name="length"> The number of characters in the hexidecimal number. </param>
        /// <returns> The numeric value of the hexidecimal number, or <c>-1</c> if the number
        /// is not valid. </returns>
        private static int ParseHexNumber(string input, int start, int length)
        {
            if (start + length > input.Length)
                return -1;
            int result = 0;
            for (int i = start; i < start + length; i++)
            {
                result *= 0x10;
                char c = input[i];
                if (c >= '0' && c <= '9')
                    result += c - '0';
                else if (c >= 'A' && c <= 'F')
                    result += c - 'A' + 10;
                else if (c >= 'a' && c <= 'f')
                    result += c - 'a' + 10;
                else
                    return -1;
            }
            return result;
        }

        /// <summary>
        /// Ref: https://github.com/paulbartrum/jurassic/blob/1e9b24b4926740aa2c1b0df8169398b3d340f681/Jurassic/Library/GlobalObject.cs#L305
        /// Decodes a string containing a URI or a portion of a URI.
        /// </summary>
        /// <param name="engine"> The script engine used to create the error objects. </param>
        /// <param name="input"> The string to decode. </param>
        /// <param name="reservedSet"> A string containing the set of characters that should not
        /// be escaped.  Alphanumeric characters should not be included. </param>
        /// <returns> A copy of the given string with the escape sequences decoded. </returns>
        internal static string DecodeURI(string input)
        {

            if (decodeURIComponentReservedSet == null)
            {
                var lookupTable = CreateCharacterSetLookupTable(";/?:@&=+$,#");
                System.Threading.Thread.MemoryBarrier();
                decodeURIComponentReservedSet = lookupTable;
            }

            var reservedSet = decodeURIComponentReservedSet;

            var result = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '%')
                {
                    // 2 digit escape sequence %XX.

                    // Decode the %XX encoding.
                    int utf8Byte = ParseHexNumber(input, i + 1, 2);
                    if (utf8Byte < 0)
                        throw JSContext.Current.NewURIError( "URI malformed");
                    i += 2;

                    // If the high bit is not set, then this is a single byte ASCII character.
                    if ((utf8Byte & 0x80) == 0)
                    {
                        // Decode only if the character is not reserved.
                        if (reservedSet[utf8Byte] == true)
                        {
                            // Leave the escape sequence as is.
                            result.Append(input.Substring(i - 2, 3));
                        }
                        else
                        {
                            result.Append((char)utf8Byte);
                        }
                    }
                    else
                    {
                        // Otherwise, this character was encoded to multiple bytes.

                        // Check for an invalid UTF-8 start value.
                        if (utf8Byte == 0xc0 || utf8Byte == 0xc1)
                            throw JSContext.Current.NewURIError( "URI malformed");

                        // Count the number of high bits set (this is the number of bytes required for the character).
                        int utf8ByteCount = 1;
                        for (int j = 6; j >= 0; j--)
                        {
                            if ((utf8Byte & (1 << j)) != 0)
                                utf8ByteCount++;
                            else
                                break;
                        }
                        if (utf8ByteCount < 2 || utf8ByteCount > 4)
                            throw JSContext.Current.NewURIError( "URI malformed");

                        // Read the additional bytes.
                        byte[] utf8Bytes = new byte[utf8ByteCount];
                        utf8Bytes[0] = (byte)utf8Byte;
                        for (int j = 1; j < utf8ByteCount; j++)
                        {
                            // An additional escape sequence is expected.
                            if (i >= input.Length - 1 || input[++i] != '%')
                                throw JSContext.Current.NewURIError( "URI malformed");

                            // Decode the %XX encoding.
                            utf8Byte = ParseHexNumber(input, i + 1, 2);
                            if (utf8Byte < 0)
                                throw JSContext.Current.NewURIError( "URI malformed");

                            // Top two bits must be 10 (i.e. byte must be 10XXXXXX in binary).
                            if ((utf8Byte & 0xC0) != 0x80)
                                throw JSContext.Current.NewURIError( "URI malformed");

                            // Store the byte.
                            utf8Bytes[j] = (byte)utf8Byte;

                            // Update the character position.
                            i += 2;
                        }

                        // Decode the UTF-8 sequence.
                        result.Append(System.Text.Encoding.UTF8.GetString(utf8Bytes, 0, utf8Bytes.Length));
                    }
                }
                else
                    result.Append(c);
            }
            return result.ToString();
        }
    }
}
