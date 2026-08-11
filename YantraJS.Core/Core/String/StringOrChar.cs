using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core;

/**
 * Replacing StringBuilder will be done in future. Right now JSString instance can manage StringBuilder if needed.
 * **/

/// <summary>
/// FastString reduces allocations by storing string or a single char. if string is null, it means it stores a single
/// character. Empty will reference back to string.Empty instance.
/// </summary>
public readonly struct StringOrChar
{

    public static readonly StringOrChar Empty = new StringOrChar(string.Empty);

    //public static implicit operator StringOrChar(string source)
    //{
    //    return new StringOrChar(source);
    //}


    // this stores single character...
    private readonly char char0;

    private readonly string @string;

    public StringOrChar(char ch)
    {
        char0 = ch;
    }

    public StringOrChar(string text)
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

    public StringOrChar Trim()
    {
        if (@string != null)
        {
            return new StringOrChar(@string.Trim());
        }
        if(char.IsWhiteSpace(char0))
        {
            return Empty;
        }
        return this;
    }

    public StringOrChar Substring(int start)
    {
        if (@string != null)
        {
            return new StringOrChar(@string.Substring(start));
        }
        if(start > 0)
        {
            return Empty;
        }
        return this;
    }

    public bool Greater(StringOrChar right)
    {
        var rightString = right.@string;
        var rightChar= right.char0;
        if (@string !=null)
        {
            if(rightString != null)
            {
                return @string.Greater(rightString);
            }
            return @string[0] > rightChar;
        }
        if(rightString != null)
        {
            return char0 > rightString[0];
        }
        return char0 > rightChar;
    }

}

public static class StringOrCharExtensions
{
    public static StringOrChar AsStringOrChar(this string value)
    {
        return new StringOrChar(value);
    }
}