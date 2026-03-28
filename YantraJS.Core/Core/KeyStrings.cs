using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core
{

    public class KeyStrings
    {

        public static readonly KeyStrings Instance = new KeyStrings();

        KeyStrings()
        {
            map = ConcurrentStringMap<KeyString>.Create();
            names = ConcurrentUInt32Map<StringSpan>.Create();
            var t = typeof(KeyString);
            foreach (var f in Enum.GetNames(t))
            {
                var k = (KeyString)Enum.Parse(t, f);
                map[f] = k;
                names[(uint)k] = f;
                NextID = (int)k;
            }
        }

        private ConcurrentStringMap<KeyString> map;
        private ConcurrentUInt32Map<StringSpan> names;

        private static int NextID = 1;

        // internal static (int size, int total, int next) Total => (map.Size, map.Total, NextID);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyString GetOrCreate(in StringSpan key)
        {
            return map.GetOrCreate(key, (keyName) =>
            {
                var i = (uint)Interlocked.Increment(ref NextID);
                names[i] = keyName;
                return (KeyString)i;
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryGet(in StringSpan key, out KeyString ks)
        {
            return map.TryGetValue(key, out ks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal KeyString GetName(uint id)
        {
            return (KeyString)id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal StringSpan GetNameString(uint id)
        {
            return names[id];
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSString GetJSString(uint id)
        {
            var name = GetNameString(id);
            return new JSString(name, (KeyString)id);
        }

    }

    public static class KeyStringsExtension
    {
        public static KeyString ToKeyString(this string key)
        {
            return KeyStrings.Instance.GetOrCreate(key);
        }

        public static KeyString ToKeyString(in this StringSpan key)
        {
            return KeyStrings.Instance.GetOrCreate(in key);
        }

        public static JSValue ToJSValue(this KeyString key)
        {
            return KeyStrings.Instance.GetJSString((uint)key);
        }
    }
}
