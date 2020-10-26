using System;

namespace WebAtoms.CoreJS.Core.Storage
{
    internal class ConcurrentTypeTrie<T>
    {

        readonly Func<Type, T> factory;
        readonly ConcurrentUInt32Trie<T> cache = new ConcurrentUInt32Trie<T>();

        public ConcurrentTypeTrie(Func<Type, T> factory)
        {
            this.factory = factory;
        }

        public T this[Type key]
        {
            get
            {
                var k = ConcurrentTypeCache.GetOrCreate(key);
                return cache.GetOrCreate(k, () => factory(key));
            }
        }
    }
}
