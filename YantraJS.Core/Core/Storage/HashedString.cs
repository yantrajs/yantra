using System;

namespace YantraJS.Core.Core.Storage
{
    public readonly struct HashedString : IEquatable<HashedString>, IComparable<HashedString>
    {
        public readonly StringSpan Value;

        public readonly int Hash;

        public HashedString(in StringSpan value)
        {
            this.Value = value;
            this.Hash = value.GetHashCode();
        }

        public HashedString(string value)
        {
            this.Value = value;
            this.Hash = Value.GetHashCode();
        }


        public static implicit operator HashedString(string v)
        {
            return new HashedString(v);
        }

        public static implicit operator HashedString(in StringSpan v)
        {
            return new HashedString(v);
        }


        public static bool operator ==(HashedString left, HashedString right)
        {
            return left.Hash == right.Hash && left.Value == right.Value;
        }

        public static bool operator !=(HashedString left, HashedString right)
        {
            return left.Hash != right.Hash || left.Value != right.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is HashedString @string && Equals(@string);
        }

        public bool Equals(HashedString other)
        {
            return Hash == other.Hash && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Hash;
        }

        public int CompareTo(HashedString other)
        {
            return Value.CompareTo(in other.Value);
        }
        public int CompareToRef(in HashedString other)
        {
            return Value.CompareTo(in other.Value);
        }

        public override string ToString()
        {
            return Value.Value;
        }
    }
}
