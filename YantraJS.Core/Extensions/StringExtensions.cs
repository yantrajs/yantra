using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace YantraJS
{
    internal static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool Greater(this string left, string right) {
            return string.CompareOrdinal(left, right) > 0;
        }

        public static bool GreaterOrEqual(this string left, string right)
        {
            return string.CompareOrdinal(left, right) >= 0;
        }


        public static bool Less(this string left, string right)
        {
            return string.CompareOrdinal(left, right) < 0;
        }

        public static bool LessOrEqual(this string left, string right)
        {
            return string.CompareOrdinal(left, right) <= 0;
        }


        public static string ToCamelCase(this string text)
        {
            int i = 0;
            foreach (char ch in text)
            {
                if (char.IsUpper(ch))
                {
                    i++;
                    continue;
                }
                break;
            }
            return text.Substring(0, i).ToLower() + text.Substring(i);
        }

    }
}
