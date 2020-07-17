using System;
using System.Collections.Generic;
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
        public readonly static JSString prototype;
        public readonly static JSString apply;
        public readonly static JSString call;

        static KeyStrings()
        {
            toString = GetOrCreate("toString");
            name = GetOrCreate("name");
            prototype = GetOrCreate("prototype");
            Number = GetOrCreate("Number");
            Object = GetOrCreate("Object");
            String = GetOrCreate("String");
            Array = GetOrCreate("Array");
            Function = GetOrCreate("Function");
            apply = GetOrCreate("apply");
            call = GetOrCreate("call");
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
