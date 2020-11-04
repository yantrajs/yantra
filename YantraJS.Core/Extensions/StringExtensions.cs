using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YantraJS
{
    internal static class StringExtensions
    {

        public static string ToCamelCase(this string text)
        {
            int i = 0;
            foreach (char ch in text)
            {
                if (Char.IsUpper(ch))
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
