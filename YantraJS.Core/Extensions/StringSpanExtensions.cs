#nullable enable
using System;
using System.Text;

namespace YantraJS.Core
{
    public static class StringSpanExtensions
    {
        public static StringSpan ToStringSpan(this string text, int offset, int length) => new StringSpan(text, offset, length);

        public unsafe static void Append(this StringBuilder sb, in StringSpan span)
        {
            fixed (char* start = span.Source)
            {
                char* ch1 = start + (span.Offset);
                sb.Append(ch1, span.Length);
            }
        }

        public static string ToSnakeCase(this StringSpan text, string prefix = "")
        {
            var sb = new StringBuilder(text.Length + prefix.Length);
            if (prefix.Length > 0) {
                sb.Append(prefix);
            }
            foreach(var ch in text)
            {
                if (char.IsUpper(ch))
                {
                    sb.Append('-');
                    sb.Append(Char.ToLower(ch));
                    continue;
                }
                sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}
