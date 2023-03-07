using System;
using System.Threading;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core.Storage
{
    internal static class ConcurrentTypeCache
    {
        private static int nextId = 0;
        private static ConcurrentStringMap<uint> cache = ConcurrentStringMap<uint>.Create();

        public static uint GetOrCreate(Type name, string suffix = "")
        {
            return cache.GetOrCreate(name.GetHashCode() + ":" + name.FullName + suffix, (_) => (uint)Interlocked.Increment(ref nextId));
        }

        // public static bool TryGetValue(Type name, out uint key) => cache.TryGetValue(name.GetHashCode() + ":" + name.FullName, out key);


    }
}
