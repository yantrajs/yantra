using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

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

    internal static class KeyStrings
    {
        public readonly static KeyString Number;
        public readonly static KeyString Object;
        public readonly static KeyString String;
        public readonly static KeyString Array;
        public readonly static KeyString Function;
        public readonly static KeyString Boolean;
        public readonly static KeyString Math;
        public readonly static KeyString JSON;
        public readonly static KeyString Date;
        public readonly static KeyString Map;
        public readonly static KeyString toString;
        public readonly static KeyString valueOf;
        public readonly static KeyString name;
        public readonly static KeyString length;
        public readonly static KeyString prototype;
        public readonly static KeyString constructor;
        public readonly static KeyString configurable;
        public readonly static KeyString enumerable;
        public readonly static KeyString @readonly;
        public readonly static KeyString apply;
        public readonly static KeyString call;
        public readonly static KeyString bind;
        public readonly static KeyString native;
        public readonly static KeyString value;
        public readonly static KeyString get;
        public readonly static KeyString set;
        public readonly static KeyString __proto__;
        public readonly static KeyString undefined;
        public readonly static KeyString NaN;
        public readonly static KeyString @null;

        static KeyStrings()
        {

            KeyString Create(string key)
            {
                var i = NextID++;
                var js = new KeyString(key, (uint)i);
                map.Save(key, js);
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
            value = Create("value");
            valueOf = Create("valueOf");
            get = Create("get");
            set = Create("set");
            Date = Create("Date");
            Map = Create("Map");
            configurable = Create("configurable");
            enumerable = Create("enumerable");
            @readonly = Create("readonly");
            undefined = Create("undefined");
            NaN = Create("NaN");
            @null = Create("null");
            Math = Create("Math");
            JSON = Create("JSON");
        }

        private static BinaryCharMap<KeyString> map = new BinaryCharMap<KeyString>();

        private static int NextID = 1;

        public static (int size, int total, int next) Total => (map.Size, map.Total, NextID);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KeyString GetOrCreate(string key)
        {
            lock(map) return map.GetOrCreate(key, () =>
            {
                var i = (uint)Interlocked.Increment(ref NextID);
                return new KeyString(key, i);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static KeyString NewSymbol(string name)
        {
            return new KeyString($"Symbol({name})", (uint)Interlocked.Increment(ref NextID));
        }

    }
}
