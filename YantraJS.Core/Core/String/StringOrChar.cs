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
            return char0 == right.@string[0];
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
        if(test.Length == 1 && char0 == test[0])
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
        if (test.Length == 1 && char0 == test[0])
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
        if (test.Length == 1 && char0 == test[0])
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
        if (test.Length == 1 && char0 == test[0])
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
        return new StringOrChar($"{char0}{new string(ch, length)}");
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
        return new StringOrChar($"{new string(ch, length)}{char0}");
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
        if(test.Length == 1 && char0 == test[0])
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
        if (test.Length == 1 && char0 == test[0])
        {
            return true;
        }
        return false;
    }

    public int Compare(int endPosition, StringOrChar test, int startPosition, int length)
    {
        if (@string != null)
        {
            if(test.@string != null)
            {
                return string.Compare(@string, endPosition, test.@string, startPosition, length);
            }
            return char0 - @string[endPosition];
        }
        if(test.IsEmpty())
        {
            return -char0;
        }
        return char0 - test[0];
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

    internal static StringOrChar Concat(BigInteger value, StringOrChar text)
    {
        if(text.IsEmpty())
        {
            return value.ToString().AsStringOrChar();
        }
        if(text.IsChar) {
            return new StringOrChar($"{value}{text.char0}");
        }
        return new StringOrChar($"{value}{text.@string}");
    }
    internal static StringOrChar Concat<T>(T value, char p1, StringOrChar text)
    {
        if (text.IsChar)
        {
            return new StringOrChar($"{value}{p1}{text.char0}");
        }
        return new StringOrChar($"{value}{p1}{text.@string}");
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
                return $"{t1.char0}{t2.char0}".AsStringOrChar();
            }
            return $"{t1.char0}{t2.@string}".AsStringOrChar();
        }
        if(t2.IsChar)
        {
            return $"{t1.@string}{t2.char0}".AsStringOrChar();
        }
        return $"{t1.@string}{t2.@string}".AsStringOrChar();
    }

    internal static StringOrChar Concat(StringOrChar t1, string t2)
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
            return $"{t1.char0}{t2}".AsStringOrChar();
        }
        return $"{t1.@string}{t2}".AsStringOrChar();
    }


    internal static StringOrChar Concat<T>(T value, StringOrChar text)
    {
        if (text.IsChar)
        {
            return new StringOrChar($"{value}{text.char0}");
        }
        return new StringOrChar($"{value}{text.@string}");
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