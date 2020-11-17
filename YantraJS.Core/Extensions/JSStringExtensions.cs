using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Extensions
{
    public static class JSStringExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Left(this string value, int max)
        {
            return value.Length > max ? value.Substring(0, max) : value;
        }

        
        public static string JSTrim(this string text)
        {
            return text.Trim();
        }

        public static string JSTrim(this JSValue text)
        {
            return text.ToString().Trim();
        }
    }
}
