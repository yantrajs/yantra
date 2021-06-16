using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace YantraJS.Core
{

    public enum KeyType
    {
        Empty = 0,
        UInt = 1,
        String = 2,
        Symbol = 3
    }

    public readonly struct PropertyKey
    {
        public readonly KeyType Type;
        public readonly uint Index;
        public readonly KeyString KeyString;
        public readonly JSSymbol Symbol;

        public bool IsUInt => Type == KeyType.UInt;

        public bool IsSymbol => Type == KeyType.Symbol;

        private PropertyKey(KeyType type, uint index,in KeyString key, JSSymbol symbol = null) {
            this.Type = type;
            this.Index = index;
            this.KeyString = key;
            this.Symbol = symbol;
        }
        public static implicit operator PropertyKey(int index)
        {
            return new PropertyKey(KeyType.UInt, (uint)index, KeyString.Empty);
        }

        public static implicit operator PropertyKey(uint index)
        {
            return new PropertyKey(KeyType.UInt, index, KeyString.Empty);
        }

        public static implicit operator PropertyKey(in KeyString key)
        {
            return new PropertyKey(KeyType.String, 0, key);
        }

        public static implicit operator PropertyKey(string key)
        {
            return new PropertyKey(KeyType.String, 0, KeyStrings.GetOrCreate(key));
        }

        public static implicit operator PropertyKey(JSSymbol key)
        {
            return new PropertyKey(KeyType.Symbol, key.Key.Key, KeyString.Empty, key);
        }
    }


    [DebuggerDisplay("Key:{Key},{Value}")]
    public readonly struct KeyString
    {

        public readonly static KeyString Empty = new KeyString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator KeyString(string value)
        {
            return KeyStrings.GetOrCreate(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator KeyString(in StringSpan value)
        {
            return KeyStrings.GetOrCreate(value);
        }


        private readonly KeyType Type;
        public readonly StringSpan Value;
        public readonly uint Key;
        public readonly JSValue JSValue;

        public bool HasValue
        {
            get
            {
                return Type != KeyType.Empty;
            }
        }

        public bool IsSymbol
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Type == KeyType.Symbol;
            }
        }

        public bool IsString
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Type == KeyType.String;
            }
        }

        public bool IsUInt
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Type == KeyType.UInt;
            }
        }

        internal KeyString(uint key)
        {
            Type = KeyType.UInt;
            this.Value = null;
            this.Key = key;
            this.JSValue = null;
        }


        internal KeyString(in StringSpan value, uint key)
        {
            Type = KeyType.String;
            this.Value = value;
            this.Key = key;
            this.JSValue = null;
        }


        internal KeyString(in StringSpan value, uint key, JSString @string)
        {
            Type = KeyType.String;
            this.Value = value;
            this.Key = key;
            this.JSValue = @string;
        }

        internal KeyString(in StringSpan value, uint key, JSSymbol symbol)
        {
            Type = KeyType.String;
            this.Value = value;
            this.Key = key;
            this.JSValue = symbol;
        }

        public override bool Equals(object obj)
        {
            if (obj is KeyString k)
                return Key == k.Key && Type == k.Type && JSValue == k.JSValue;
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
            return Value.Value;
        }

        public JSValue ToJSValue()
        {
            if (JSValue != null)
                return JSValue;
            return new JSString(Value, this);
        }

        // public static (int size, int total, int next) Total =>
            // KeyStrings.Total;

    }
}
