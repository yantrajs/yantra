using System;
using System.Runtime.CompilerServices;

namespace YantraJS.Core.FastParser
{
    internal static class CharExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string FromCodePoint(this int cp)
        {
            return Char.ConvertFromUtf32(cp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int HexValue(this char ch)
        {
            if (ch >= 'A')
            {
                if (ch >= 'a')
                {
                    if (ch <= 'f')
                    {
                        return ch - 'a' + 10;
                    }
                }
                else if (ch <= 'F')
                {
                    return ch - 'A' + 10;
                }
            }
            else if (ch <= '9')
            {
                return ch - '0';
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsDigitPart(
            this char ch)
        {
            switch (ch)
            {
                case '_':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;
            }
            return false;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsHexDigitPart(
            this char ch)
        {
            switch (ch)
            {
                case '_':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsHexDigitPart(
            this char ch, out int v)
        {
            switch (ch)
            {
                case '_':
                    v = 0;
                    return true;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    v = (int)ch - '0';
                    return true;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    v = 10 + (int)ch - 'a';
                    return true;
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    v = 10 + (int)ch - 'A';
                    return true;
            }
            v = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsBinaryDigitPart(
            this char ch)
        {
            switch (ch)
            {
                case '_':
                case '0':
                case '1':
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsIdentifierStart(this char ch)
        {
            switch (ch)
            {
                case '_':
                case '$':
                case '@':
                    return true;
            }
            return char.IsLetter(ch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsIdentifierPart(this char ch)
        {
            switch (ch)
            {
                case '_':
                case '$':
                case '@':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;
            }
            return char.IsLetter(ch);
        }
    }
}
