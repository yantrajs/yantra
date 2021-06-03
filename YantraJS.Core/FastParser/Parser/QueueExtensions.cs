#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace YantraJS.Core.FastParser
{
    public static class QueueExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey,TValue> dictionary, 
            TKey key,
            Func<TKey, TValue> factory)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;
            value = factory(key);
            dictionary[key] = value;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        
        public static bool TryDequeue<T>(this Queue<T> queue, out T value)
        {
            if (queue.Count > 0)
            {
                value = queue.Dequeue();
                return true;
            }
#pragma warning disable CS8601 // Possible null reference assignment.
            value = default;
#pragma warning restore CS8601 // Possible null reference assignment.
            return false;
        }
    }
}
