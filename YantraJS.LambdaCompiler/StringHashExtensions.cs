#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YantraJS.Internals
{
    public static class StringHashExtensions
    {
        public static bool HashMatch(string? text, int hashText, string? source, int hashSource)
        {
            if (hashText != hashSource)
                return false;
            return source == text;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static unsafe int UnsafeGetHashCode(this string? Source, int Offset = 0, int Length = 0)
        {
            unchecked
            {
                if (Source == null || Source.Length == 0)
                    return 0;
                if (Length == 0)
                    Length = Source.Length;
                fixed (char* src = Source)
                {
                    int hash1 = 5381;
                    int hash2 = hash1;

                    int c;
                    char* s = src + Offset;
                    c = s[0];
                    for (int i = 0; i < Length; i++)
                    {
                        c = s[i];
                        hash1 = ((hash1 << 5) + hash1) ^ c;
                        if (i == Length - 1)
                            break;
                        c = s[i + 1];
                        hash2 = ((hash2 << 5) + hash2) ^ c;
                    }
                    //while ((c = s[0]) != 0)
                    //{
                    //    hash1 = ((hash1 << 5) + hash1) ^ c;
                    //    c = s[1];
                    //    if (c == 0)
                    //        break;
                    //    hash2 = ((hash2 << 5) + hash2) ^ c;
                    //    s += 2;
                    //}
                    return hash1 + (hash2 * 1566083941);
                }
            }
        }
    }
}
