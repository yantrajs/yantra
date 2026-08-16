using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace YantraJS.Core;

/**
 * Replacing StringBuilder will be done in future. Right now JSString instance can manage StringBuilder if needed.
 * **/

/// <summary>
/// FastString reduces allocations by storing string or a single char. if string is null, it means it stores a single
/// character. Empty will reference back to string.Empty instance.
/// </summary>
public readonly struct StringOrChar: IEnumerable<char>
{

    public static readonly StringOrChar Empty = new StringOrChar(string.Empty);


    public static bool operator ==(in StringOrChar left, in StringOrChar right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(in StringOrChar left, in StringOrChar right)
    {
        return !left.Equals(right);
    }

    //public static implicit operator StringOrChar(string source)
    //{
    //    return new StringOrChar(source);
    //}


    // this stores single character...
    private readonly char char0;

    private readonly string @string;

    public bool IsChar => @string == null;

    public int Length => @string?.Length ?? 1;

    public bool IsEmpty() => @string?.IsEmpty() ?? false;

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

    public bool Equals(in StringOrChar right)
    {
        if(this.Length != right.Length)
        {
            return false;
        }
        if(@string != null)
        {
            if(right.@string != null)
            {
                return @string.Equals(right.@string);
            }
            return @string.Equals(char0);
        }
        if (right.@string != null)
        {
            return right.@string.Equals(char0);
        }
        return char0 == right.char0;
    }

    public int CompareTo(in StringOrChar right)
    {
        if (@string != null)
        {
            if (right.@string != null)
            {
                return @string.CompareTo(right.@string);
            }
            return @string.CompareTo(char0);
        }
        if (right.@string != null)
        {
            return right.@string.CompareTo(char0);
        }
        return char0.CompareTo(right.char0);
    }

    public int CompareTo(string right)
    {
        return CompareTo(right.AsStringOrChar());
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

    public bool GreaterOrEqual(StringOrChar right)
    {
        var rightString = right.@string;
        var rightChar = right.char0;
        if (@string != null)
        {
            if (rightString != null)
            {
                return @string.GreaterOrEqual(rightString);
            }
            return @string[0] >= rightChar;
        }
        if (rightString != null)
        {
            return char0 >= rightString[0];
        }
        return char0 >= rightChar;
    }

    public bool Less(StringOrChar right)
    {
        var rightString = right.@string;
        var rightChar = right.char0;
        if (@string != null)
        {
            if (rightString != null)
            {
                return @string.Less(rightString);
            }
            return @string[0] < rightChar;
        }
        if (rightString != null)
        {
            return char0 < rightString[0];
        }
        return char0 < rightChar;
    }

    public bool LessOrEqual(StringOrChar right)
    {
        var rightString = right.@string;
        var rightChar = right.char0;
        if (@string != null)
        {
            if (rightString != null)
            {
                return @string.LessOrEqual(rightString);
            }
            return @string[0] <= rightChar;
        }
        if (rightString != null)
        {
            return char0 <= rightString[0];
        }
        return char0 <= rightChar;
    }

    public override string ToString()
    {
        if(@string != null)
        {
            return @string;
        }
        return new string(this.char0, 1);
    }

    internal StringOrChar Add(StringOrChar value)
    {
        if(this.IsEmpty())
        {
            return value;
        }
        if (value.IsEmpty())
        {
            return this;
        }
        if (@string != null)
        {
            if(value.@string != null)
            {
                return (@string + value.@string).AsStringOrChar();
            }
            return $"{@string}{value.char0}".AsStringOrChar();            
        }
        if(value.@string != null)
        {
            return $"{char0}{value.@string}".AsStringOrChar();
        }
        return $"{char0}{value.char0}".AsStringOrChar();
    }

    internal StringOrChar Add(double value)
    {
        if(this.IsEmpty())
        {
            return value.ToString().AsStringOrChar();
        }
        if(@string != null)
        {
            return (@string + value).AsStringOrChar();
        }
        return $"{char0}{value}".AsStringOrChar();
    }

    internal StringOrChar Add(string value)
    {
        if (this.IsEmpty())
        {
            return value.ToString().AsStringOrChar();
        }
        if(value.IsEmpty())
        {
            return this;
        }
        if (@string != null)
        {
            return (@string + value).AsStringOrChar();
        }
        return $"{char0}{value}".AsStringOrChar();
    }

    public IEnumerator<char> GetEnumerator()
    {
        if(@string != null) {  return @string.GetEnumerator(); }
        return new CharEnumerator(char0);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    internal readonly struct CharEnumerable : IEnumerable<char>
    {
        private readonly char char0;

        public CharEnumerable(char char0)
        {
            this.char0 = char0;
        }

        public IEnumerator<char> GetEnumerator()
        {
            return new CharEnumerator(this.char0);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CharEnumerator(this.char0);
        }
    }

    internal struct CharEnumerator: IEnumerator<char>
    {
        private readonly char char0;
        private bool read;

        public CharEnumerator(char char0)
        {
            this.char0 = char0;
        }

        public char Current => char0;

        object IEnumerator.Current => char0;

        public void Dispose()
        {
            this.read = false;
        }

        public bool MoveNext()
        {
            if(this.read)
            {
                return false;
            }
            this.read = true;
            return true;
        }

        public void Reset()
        {
            this.read = false;
        }
    }

}

public static class StringOrCharExtensions
{
    public static StringOrChar AsStringOrChar(this string value)
    {
        return new StringOrChar(value);
    }
}