using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core;

/**
 * The reason this class exists is to not use pooled string builders.
 * Most of string mutations contain small strings that can fit in stack.
 * Most of parsing actually uses single characters as tokens.
 * **/

/// <summary>
/// FastString reduces allocations by providing 3 storage ways. Appendable strings, Single character, and 
/// original string span. (Offset field is only used with respect to string/char array).
/// </summary>
public readonly struct FastString
{

    public static readonly FastString Empty = new FastString();

    public readonly int Length;
    public readonly int Offset;

    // this stores single character...
    private readonly char char0;

    //private readonly char char1;
    //private readonly char char2;
    //private readonly char char3;

    private readonly char[] chars;
    private readonly string @string;

    public FastString(char ch)
    {
        Length = 1;
        char0 = ch;
    }

    public FastString(string text)
    {
        Length = text.Length;
        if (Length == 1)
        {
            char0 = text[0];
            return;
        }
        @string = text;
    }

    public FastString(char[] text)
    {
        Length = text.Length;
        if (Length == 1)
        {
            char0 = text[0];
            return;
        }
        chars = text;
    }

    public char this[int index]
    {   get { 
            if(@string != null)
            {
                return @string[index-Offset];
            }
            if (chars != null)
            {
                return chars[index-Offset];
            }
            if(index == 0 )
            {
                return char0;
            }
            throw new ArgumentOutOfRangeException();
        }
    }

    public FastString Trim()
    {
        if (@string != null)
        {
            return new FastString(@string.Trim());
        }
        if (chars != null)
        {
            
        }
    }

    public FastString Substring(int start)
    {
        if (@string != null)
        {
            if(start == @string.Length - 1)
            {
                return Empty;
            }
        }
    }

}
