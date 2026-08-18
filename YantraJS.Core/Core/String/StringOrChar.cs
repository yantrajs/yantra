using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
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
        return left.Equals(in right);
    }
    public static bool operator !=(in StringOrChar left, in StringOrChar right)
    {
        return !left.Equals(in right);
    }

    //public static implicit operator StringOrChar(string source)
    //{
    //    return new StringOrChar(source);
    //}


    // this stores single character...
    private readonly char char0;

    private readonly string @string;

    public bool IsChar => @string == null;

    public char Char => char0;

    /** The reason this exists is, jit can inline this for fast compare*/
    internal char FirstChar => @string?[0] ?? char0;

    public string String => @string;

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
            throw new ArgumentOutOfRangeException($"{index}");
        }
    }

    public bool Equals(string right)
    {
        return Equals(new StringOrChar(right));
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
            return @string[0] == right.char0;
        }
        if (right.Length == 1)
        {
            return char0 == right.FirstChar;
        }
        return false;
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

    public StringOrChar Trim(char[] chars)
    {
        if (@string != null)
        {
            return new StringOrChar(@string.Trim(chars));
        }
        if(chars.Contains(char0))
        {
            return Empty;
        }
        return this;
    }


    public StringOrChar TrimStart(char[] chars)
    {
        if (@string != null)
        {
            return new StringOrChar(@string.TrimStart(chars));
        }
        if (chars.Contains(char0))
        {
            return Empty;
        }
        return this;
    }

    public StringOrChar TrimEnd(char[] chars)
    {
        if (@string != null)
        {
            return new StringOrChar(@string.TrimEnd(chars));
        }
        if (chars.Contains(char0))
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

    public StringOrChar Substring(int start, int length)
    {
        if (@string != null)
        {
            return new StringOrChar(@string.Substring(start, length));
        }
        if (start > 0)
        {
            return Empty;
        }
        return this;
    }

    public int IndexOf(StringOrChar test)
    {
        if(@string != null)
        {
            if(test.@string != null)
            {
                return @string.IndexOf(test.@string);
            }
            return @string.IndexOf(test.char0);
        }
        if(test.Length == 1 && char0 == test.FirstChar)
        {
            return 0;
        }
        return -1;
    }

    public int IndexOf(StringOrChar test, int position)
    {
        if (@string != null)
        {
            if (test.@string != null)
            {
                return @string.IndexOf(test.@string, position);
            }
            return @string.IndexOf(test.char0, position);
        }
        if(position > 0)
        {
            return -1;
        }
        if (test.Length == 1 && char0 == test.FirstChar)
        {
            return 0;
        }
        return -1;
    }

    public int LastIndexOf(StringOrChar test, int position)
    {
        if (@string != null)
        {
            if (test.@string != null)
            {
                return @string.LastIndexOf(test.@string, position);
            }
            return @string.LastIndexOf(test.char0, position);
        }
        if (position > 0)
        {
            return -1;
        }
        if (test.Length == 1 && char0 == test.FirstChar)
        {
            return 0;
        }
        return -1;
    }


    public bool EndsWith(StringOrChar test)
    {
        if (@string != null)
        {
            if (test.@string != null)
            {
                return @string.EndsWith(test.@string);
            }
            return @string.IndexOf(test.char0) == @string.Length-1;
        }
        if (test.Length == 1 && char0 == test.FirstChar)
        {
            return true;
        }
        return false;
    }

    public StringOrChar Normalize(NormalizationForm nf)
    {
        if(@string != null)
        {
            return @string.Normalize(nf).AsStringOrChar();
        }
        return this.ToString().Normalize(nf).AsStringOrChar();
    }

    public StringOrChar PadRight(int length, char ch)
    {
        if (@string != null)
        {
            return @string.PadRight(length, ch).AsStringOrChar();
        }
        return new StringOrChar(char0 + new string(ch, length));
    }

    public StringOrChar ToLowerInvariant()
    {
        if(@string != null)
        {
            return @string.ToLowerInvariant().AsStringOrChar();
        }
        return new StringOrChar(Char.ToLowerInvariant(char0));
    }


    public StringOrChar ToUpperInvariant()
    {
        if (@string != null)
        {
            return @string.ToUpperInvariant().AsStringOrChar();
        }
        return new StringOrChar(Char.ToUpperInvariant(char0));
    }

    public StringOrChar ToLower(CultureInfo culture)
    {
        if (@string != null)
        {
            return @string.ToLower(culture).AsStringOrChar();
        }
        return new StringOrChar(Char.ToLower(char0, culture));
    }

    public StringOrChar ToUpper(CultureInfo culture)
    {
        if (@string != null)
        {
            return @string.ToUpper(culture).AsStringOrChar();
        }
        return new StringOrChar(Char.ToUpper(char0, culture));
    }

    public StringOrChar PadLeft(int length, char ch)
    {
        if (@string != null)
        {
            return @string.PadLeft(length, ch).AsStringOrChar();
        }
        return new StringOrChar(new string(ch, length) + char0);
    }

    public StringOrChar Replace(StringOrChar test, StringOrChar replace)
    {
        if (@string != null)
        {
            var testString = test.@string;
            if(testString != null)
            {
                return @string.Replace(testString, replace.ToString()).AsStringOrChar();
            }
            if(replace.IsChar)
            {
                return @string.Replace(test.char0, replace.char0).AsStringOrChar();
            }
            return @string.Replace(test.ToString(), replace.ToString()).AsStringOrChar();
        }
        // the only case is...
        if(test.Length == 1 && char0 == test.FirstChar)
        {
            return replace;
        }
        return this;
    }

    public bool StartsWith(StringOrChar test)
    {
        if (@string != null)
        {
            if (test.@string != null)
            {
                return @string.StartsWith(test.@string);
            }
            return @string[0] == test.char0;
        }
        if (test.Length == 1 && char0 == test.FirstChar)
        {
            return true;
        }
        return false;
    }

    public int CompareOrdinal(StringOrChar test)
    {
        if (@string != null && test.@string != null) {
            return string.CompareOrdinal(@string, test.@string);
        }
        var i = FirstChar - test.FirstChar;
        if (i == 0)
        {
            return Length - test.Length;
        }
        return i;
    }

    public int Compare(int endPosition, StringOrChar test, int startPosition, int length)
    {
        if (@string != null)
        {
            if(test.@string != null)
            {
                return string.Compare(@string, endPosition, test.@string, startPosition, length);
            }
            if(startPosition > 0)
            {
                return @string[endPosition];
            }
            return @string[endPosition] - test.char0;
        }
        if(test.IsEmpty())
        {
            return char0;
        }
        return char0 - test[startPosition];
    }

    public bool Greater(StringOrChar right)
    {
        if (@string != null && right.@string != null) {
            return @string.Greater(right.@string);
        }
        var selfChar0 = this.FirstChar;
        var rightChar0 = right.FirstChar;
        // if both char are same...
        var i = selfChar0 - rightChar0;
        if(i == 0)
        {
            return Length > right.Length;                
        }
        return i > 0;
    }

    public bool GreaterOrEqual(StringOrChar right)
    {
        if (@string != null && right.@string != null)
        {
            return @string.GreaterOrEqual(right.@string);
        }
        var selfChar0 = this.FirstChar;
        var rightChar0 = right.FirstChar;
        // if both char are same...
        var i = selfChar0 - rightChar0;
        if (i == 0)
        {
            return Length >= right.Length;
        }
        return i >= 0;
    }

    public bool Less(StringOrChar right)
    {
        if (@string != null && right.@string != null)
        {
            return @string.Less(right.@string);
        }
        var selfChar0 = this.FirstChar;
        var rightChar0 = right.FirstChar;
        // if both char are same...
        var i = selfChar0 - rightChar0;
        if (i == 0)
        {
            return Length < right.Length;
        }
        return i < 0;
    }

    public bool LessOrEqual(StringOrChar right)
    {
        if (@string != null && right.@string != null)
        {
            return @string.LessOrEqual(right.@string);
        }
        var selfChar0 = this.FirstChar;
        var rightChar0 = right.FirstChar;
        // if both char are same...
        var i = selfChar0 - rightChar0;
        if (i == 0)
        {
            return Length <= right.Length;
        }
        return i <= 0;

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
        return Concat(this, value);
    }

    internal StringOrChar Add(double value)
    {
        return Concat(this, value.ToString());
    }

    internal StringOrChar Add(string value)
    {
        return Concat(this, value);
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

    internal static StringOrChar Concat(BigInteger value, StringOrChar text)
    {
        if(text.IsEmpty())
        {
            return value.ToString().AsStringOrChar();
        }
        if(text.IsChar) {
            return new StringOrChar(value.ToString() + text.char0);
        }
        return new StringOrChar(value.ToString() + text.@string);
    }
    internal static StringOrChar Concat<T>(T value, char p1, StringOrChar text)
    {
        if (text.IsChar)
        {
            return new StringOrChar(value.ToString() + p1 + text.char0);
        }
        return new StringOrChar(value.ToString() + p1 + text.@string);
    }

    internal static StringOrChar Concat(StringOrChar t1, StringOrChar t2)
    {
        if (t1.IsEmpty())
        {
            return t2;
        }
        if (t2.IsEmpty())
        {
            return t1;
        }
        if (t1.IsChar)
        {
            if (t2.IsChar)
            {
                return (t1.char0.ToString() + t2.char0).AsStringOrChar();
            }
            return (t1.char0 + t2.@string).AsStringOrChar();
        }
        if(t2.IsChar)
        {
            return (t1.@string + t2.char0).AsStringOrChar();
        }
        return (t1.@string + t2.@string).AsStringOrChar();
    }

    internal unsafe static StringOrChar Concat(StringOrChar t1, string t2)
    {
        if(t1.IsEmpty())
        {
            return t2.AsStringOrChar();
        }
        if(t2.IsEmpty())
        {
            return t1;
        }
        if (t1.IsChar)
        {
            return (t1.char0 + t2).AsStringOrChar();
        }
        return (t1.@string+t2).AsStringOrChar();
    }


    internal static StringOrChar Concat<T>(T value, StringOrChar text)
    {
        if (text.IsChar)
        {
            return new StringOrChar(value + text.char0.ToString());
        }
        return new StringOrChar(value + text.@string);
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
        if(value.Length == 1)
        {
            return new StringOrChar(value[0]);
        }
        return new StringOrChar(value);
    }
}