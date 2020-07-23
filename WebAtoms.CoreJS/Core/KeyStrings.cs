using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace WebAtoms.CoreJS.Core
{

    public interface IKeyString
    {
        uint Key { get; }
    }
    public struct KeyString
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator KeyString(string value)
        {
            return KeyStrings.GetOrCreate(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator String(KeyString value)
        {
            return value.Value;
        }

        //public static bool operator == (KeyString a, KeyString b) {
        //    return a.Key == b.Key;
        //}
        //public static bool operator != (KeyString a, KeyString b)
        //{
        //    return a.Key != b.Key;
        //}

        //public static bool operator ==(KeyString a, string b)
        //{
        //    return a.Value == b;
        //}
        //public static bool operator !=(KeyString a, string b)
        //{
        //    return a.Value != b;
        //}

        public readonly string Value;
        public readonly uint Key;
        internal KeyString(string value, uint key)
        {
            this.Value = value;
            this.Key = key;
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
    }

    internal static class KeyStrings
    {
        public readonly static KeyString Number;
        public readonly static KeyString Object;
        public readonly static KeyString String;
        public readonly static KeyString Array;
        public readonly static KeyString Function;
        public readonly static KeyString Boolean;
        public readonly static KeyString toString;
        public readonly static KeyString name;
        public readonly static KeyString length;
        public readonly static KeyString prototype;
        public readonly static KeyString constructor;
        public readonly static KeyString apply;
        public readonly static KeyString call;
        public readonly static KeyString bind;
        public readonly static KeyString native;
        public readonly static KeyString __proto__;

        static KeyStrings()
        {

            KeyString Create(string key)
            {
                var i = NextID++;
                var js = new KeyString(key, (uint)i);
                map[key] = js;
                return js;
            }

            toString = Create("toString");
            name = Create("name");
            constructor = Create("constructor");
            prototype = Create("prototype");
            __proto__ = Create("__proto__");
            Number = Create("Number");
            Object = Create("Object");
            String = Create("String");
            Array = Create("Array");
            Function = Create("Function");
            length = Create("length");
            apply = Create("apply");
            call = Create("call");
            bind = Create("bind");
            native = Create("native");
            Boolean = Create("Boolean");
        }

        private static BinaryCharMap<KeyString> map = new BinaryCharMap<KeyString>();

        private static int NextID = 1;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KeyString GetOrCreate(string key)
        {
            lock(map) return map.GetOrCreate(key, () =>
            {
                var i = (uint)Interlocked.Increment(ref NextID);
                return new KeyString(key, i);
            });
        }

    }
}
