using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace YantraJS.Core.Storage
{
    //internal abstract class ConcurrentSharedStringTrie<T>: ConcurrentUInt32Trie<T>
    //{

    //    internal struct Key {
    //        internal readonly string Name;
    //        internal readonly uint Id;

    //        private Key(string name, uint id)
    //        {
    //            this.Name = name;
    //            this.Id = id;
    //        }

    //        public static implicit operator Key(string name)
    //        {
    //            return cache.GetOrCreate(name, () => new Key(name, (uint)Interlocked.Increment(ref nextId)));
    //        }

    //        internal static bool TryGetValue(string name, out Key key) => cache.TryGetValue(name, out key);
    //    }

    //    private static int nextId = 0;
    //    private static ConcurrentStringTrie<Key> cache = new ConcurrentStringTrie<Key>();

    //    public ConcurrentSharedStringTrie()
    //    {

    //    }


    //    public T this [Key key]
    //    {
    //        get => this[key.Id];
    //        set => this[key.Id] = value;
    //    }

    //    public T GetOrCreate(string key, Func<T> factory)
    //    {
    //        Key k = key;
    //        return GetOrCreate(k.Id, factory);
    //    }

    //    public bool TryGetValue(string key, out T value)
    //    {
    //        if (Key.TryGetValue(key, out var k))
    //            return TryGetValue(k.Id, out value);
    //        value = default;
    //        return false;
    //    }

    //}
}
