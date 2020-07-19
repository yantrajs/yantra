using System;

namespace WebAtoms.CoreJS.Core
{
    public struct JSName
    {
        public readonly KeyString Key;
        readonly string displayName;

        public static implicit operator JSName(KeyString input)
        {
            return new JSName(input);
        }

        public static implicit operator JSName(string input)
        {
            return new JSName(input);
        }

        public JSName(KeyString key, string displayName = null)
        {
            this.displayName = displayName ?? key.Value;
            this.Key = key;
        }

        public override bool Equals(object obj)
        {
            if (obj is JSName jn)
                return Key.Key == jn.Key.Key;
            if (obj is KeyString k)
                return Key.Key == k.Key;
            if (obj is String sv)
                return Key.Value == sv;
            return false;
        }

        public override int GetHashCode()
        {
            return (int)Key.Key;
        }

        public override string ToString()
        {
            return this.displayName;
        }
    }
}
