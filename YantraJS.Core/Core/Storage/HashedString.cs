using System;

namespace YantraJS.Core.Core.Storage
{
    public readonly struct HashedString : IEquatable<HashedString>, IComparable<HashedString>
    {
        public readonly string Value;

        public readonly int Hash;

        public HashedString(string value)
        {
            this.Value = value;
            this.Hash = value.GetHashCode();
        }

        public static implicit operator HashedString(string v)
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
            return Value.CompareTo(other.Value);
        }
        public int CompareToRef(in HashedString other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
