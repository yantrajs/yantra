using System;
using System.Runtime.CompilerServices;

namespace WebAtoms.CoreJS.Core
{
    public struct KeyString
    {

        public readonly static KeyString Empty = new KeyString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator KeyString(string value)
        {
            return KeyStrings.GetOrCreate(value);
        }

        public readonly string Value;
        public readonly uint Key;
        public readonly JSSymbol Symbol;
        public readonly JSString String;

        public bool IsSymbol
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Symbol != null;
            }
        }

        public bool IsString
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return String != null;
            }
        }

        public bool IsUInt
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Value == null;
            }
        }

        internal KeyString(string value, uint key)
        {
            this.Value = value;
            this.Key = key;
            this.String = null;
            this.Symbol = null;
        }


        internal KeyString(string value, uint key, JSString @string)
        {
            this.Value = value;
            this.Key = key;
            this.String = @string;
            this.Symbol = null;
        }

        internal KeyString(string value, uint key, JSSymbol symbol)
        {
            this.Value = value;
            this.Key = key;
            this.Symbol = symbol;
            this.String = null;
        }

        public override bool Equals(object obj)
        {
            if (obj is KeyString k)
                return Key == k.Key;
            if (obj is string sv)
                return Value == sv;
            return false;
        }

        public override int GetHashCode()
        {
            return (int)Key;
        }

        public override string ToString()
        {
            return Value;
        }

        public JSValue ToJSValue()
        {
            if (Symbol != null)
                return Symbol;
            if (String != null)
                return String;
            return new JSString(Value, this);
        }

        public static (int size, int total, int next) Total =>
            KeyStrings.Total;

    }
}
