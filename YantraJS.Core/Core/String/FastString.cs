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

    public static readonly FastString Empty = new FastString(string.Empty);

    // this stores single character...
    private readonly char char0;

    private readonly string @string;

    public FastString(char ch)
    {
        char0 = ch;
    }

    public FastString(string text)
    {
        @string = text;
    }

    public char this[int index]
    {
        get { 
            if(@string != null)
            {
                return @string[index];
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
        if(char.IsWhiteSpace(char0))
        {
            return Empty;
        }
        return this;
    }

    public FastString Substring(int start)
    {
        if (@string != null)
        {
            return new FastString(@string.Substring(start));
        }
        if(start > 0)
        {
            return Empty;
        }
        return this;
    }

}
