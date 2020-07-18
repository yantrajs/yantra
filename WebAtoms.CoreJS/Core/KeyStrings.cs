using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace WebAtoms.CoreJS.Core
{
    internal static class KeyStrings
    {
        public readonly static JSString Number;
        public readonly static JSString Object;
        public readonly static JSString String;
        public readonly static JSString Array;
        public readonly static JSString Function;
        public readonly static JSString toString;
        public readonly static JSString name;
        public readonly static JSString length;
        public readonly static JSString prototype;
        public readonly static JSString constructor;
        public readonly static JSString apply;
        public readonly static JSString call;
        public readonly static JSString bind;
        public readonly static JSString native;
        public readonly static JSString __proto__;

        static KeyStrings()
        {

            JSString Create(string key)
            {
                var i = NextID++;
                var js = new JSString(key, (uint)i);
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
        }

        private static BinaryCharMap<JSString> map = new BinaryCharMap<JSString>();

        private static int NextID = 1;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSString GetOrCreate(string key)
        {
            lock(map) return map.GetOrCreate(key, () =>
            {
                var i = (uint)Interlocked.Increment(ref NextID);
                return new JSString(key, i);
            });
        }

    }
}
