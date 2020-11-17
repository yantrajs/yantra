#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core
{
    public readonly struct StringSpan: IEquatable<StringSpan>, IEquatable<string>
    {

        public static readonly StringSpan Empty = string.Empty;

        public readonly string? Source;
        public readonly int Offset;
        public readonly int Length;
        public StringSpan(string? source)
        {
            this.Source = source;
            this.Offset = 0;
            this.Length = source?.Length ?? 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringSpan(string? buffer, int offset, int length)
        {
            if (buffer == null || (uint)offset > (uint)buffer.Length || (uint)length > (uint)(buffer.Length - offset))
            {
                throw new InvalidOperationException($"offset/length represents invalid string or string is null");
            }
            this.Source = buffer;
            this.Offset = offset;
            this.Length = length;
        }

        public string Value
        {
            get
            {
                return Source?.Substring(Offset, Length) ?? string.Empty;
            }
        }

        public char this[int index]
        {
            get
            {
                if (Source == null || (uint)index >= (uint)Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return Source[Offset + index];
            }
        }

        public static int Compare(in StringSpan a, in StringSpan b, StringComparison comparisonType)
        {
            int minLength = Math.Min(a.Length, b.Length);
            int diff = string.Compare(a.Source, a.Offset, b.Source, b.Offset, minLength, comparisonType);
            if (diff == 0)
            {
                diff = a.Length - b.Length;
            }

            return diff;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is StringSpan segment && Equals(in segment, StringComparison.Ordinal);
        }


        public override string ToString()
        {
            return Value ?? string.Empty;
        }

        public bool IsEmpty => Length == 0;

        public static implicit operator StringSpan(string source)
        {
            return new StringSpan(source);
        }

        public static bool operator ==(in StringSpan left, in StringSpan right)
        {
            return left.Equals(in right, StringComparison.Ordinal);
        }
        public static bool operator !=(in StringSpan left, in StringSpan right)
        {
            return !left.Equals(in right, StringComparison.Ordinal);
        }

        public bool Equals(StringSpan other) => Equals(other, StringComparison.Ordinal);

        public bool Equals(in StringSpan other, StringComparison comparisonType)
        {
            if (Length != other.Length)
            {
                return false;
            }

            return string.Compare(Source, Offset, other.Source, other.Offset, other.Length, comparisonType) == 0;
        }

        public static bool Equals(in StringSpan a, in StringSpan b, StringComparison comparisonType)
        {
            return a.Equals(b, comparisonType);
        }
        public bool Equals(string other)
        {
            return Equals(other, StringComparison.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(string text, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            int textLength = text.Length;
            if (Source == null || Length != textLength)
            {
                return false;
            }

            return string.Compare(Source, Offset, text, 0, textLength, comparisonType) == 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public StringSpanReader Reader()
        {
            return new StringSpanReader(this);
        }
        public string Substring(int index)
        {
            throw new NotImplementedException();
        }


    }
}
