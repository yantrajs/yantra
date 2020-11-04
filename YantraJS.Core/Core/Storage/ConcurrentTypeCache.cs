using System;
using System.Threading;

namespace YantraJS.Core.Storage
{
    internal static class ConcurrentTypeCache
    {
        private static int nextId = 0;
        private static ConcurrentStringTrie<uint> cache = new ConcurrentStringTrie<uint>();

        public static uint GetOrCreate(Type name)
        {
            return cache.GetOrCreate(name.GetHashCode() + ":" + name.FullName, () => (uint)Interlocked.Increment(ref nextId));
        }

        public static bool TryGetValue(Type name, out uint key) => cache.TryGetValue(name.GetHashCode() + ":" + name.FullName, out key);


    }
}
