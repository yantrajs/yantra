using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WebAtoms.CoreJS.Core.Storage;

namespace WebAtoms.CoreJS.Core
{
    internal abstract class BaseMap<TKey, TValue> : IBitTrie<TKey, TValue, BaseMap<TKey, TValue>.TrieNode>
        where TKey : IComparable<TKey>
    {
        protected TrieNode[] Buffer;

        private long count;

        protected uint size;
        protected uint next;
        protected readonly uint grow;

        public long Count => count;

        public BaseMap(uint size, uint grow)
        {
            this.size = size;
            this.next = size;
            this.grow = grow;
            Buffer = new TrieNode[grow];
        }


        public TValue this[TKey input]
        {
            get
            {
                if (this.TryGetValue(input, out var t))
                    return t;
                return default;
            }
            set
            {
                this.Save(input, value);
            }
        }

        /// <summary>
        /// Recursive enumerators are bad...
        /// </summary>
        public IEnumerable<(TKey Key, TValue Value)> AllValues
        {
            get
            {
                foreach (var (k, v, _) in Enumerate(0))
                {
                    yield return (k, v);
                }
            }
        }


        ////ref struct Enumerator 
        ////{
        ////    private BaseMap<TKey, TValue> map;
        ////    private uint index;
        ////    private TValue value;
        ////    public Enumerator(BaseMap<TKey,TValue> map, uint start)
        ////    {
        ////        this.map = map;
        ////        index = start;
        ////        value = default;
        ////    }

        ////    public bool MoveNext()
        ////    {
        ////        if (index < this.map.size)
        ////        {
        ////            ref var node = ref map.Buffer[index];
        ////            if (node.HasValue)
        ////            {
        ////                value = node.Value;
        ////                return true;
        ////            }
        ////        } else
        ////        {

        ////        }
        ////        index++;
        ////        return false;
        ////    }

        ////    public TValue Current => value;

        ////}

        protected abstract IEnumerable<(TKey Key, TValue Value, UInt32 index)> Enumerate(UInt32 index);

        protected int Update(Func<TKey, TValue, (bool replace, TValue value)> update, UInt32 index = 0)
        {
            int count = 0;
            var last = index + this.size;
            for (uint i = index; i < last; i++)
            {
                var node = Buffer[i];
                var fi = node.FirstChildIndex;
                if (node.HasValue)
                {
                    var uv = update(node.Key, node.Value);
                    if (uv.replace)
                    {
                        node.Update(node.Key, uv.value);
                        count++;
                    }
                    continue;
                }
                if (!node.HasIndex)
                {
                    continue;
                }
                count += Update(update, fi);
            }
            return count;
        }

        public bool TryGetKeyOf(TValue value, out TKey key)
        {
            foreach (var (k, v, _) in Enumerate(0))
            {
                if (v.Equals(value))
                {
                    key = k;
                    return true;
                }
            }
            key = default;
            return false;
        }

        public bool RemoveValue(TValue value)
        {
            foreach (var (_, v, i) in Enumerate(0))
            {
                if (v.Equals(value))
                {
                    count--;
                    Buffer[i].ClearValue();
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetOrCreate(TKey key, Func<TValue> factory)
        {
            ref var node = ref GetTrieNode(key, true);
            if (node.HasValue)
            {
                return node.Value;
            }
            var v = factory();
            node.Update(key, v);
            return v;
        }

        protected abstract ref TrieNode GetTrieNode(TKey key, bool create = false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            ref var node = ref GetTrieNode(key);
            if (node.HasValue)
            {
                value = node.Value;
                return true;
            }
            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKey(TKey key)
        {
            ref var node = ref GetTrieNode(key);
            if (node.HasValue)
            {
                return true;
            }
            return false;
        }


        public bool RemoveAt(TKey key)
        {
            ref var node = ref GetTrieNode(key, false);
            if (node.HasValue)
            {
                count--;
                node.ClearValue();
                return true;
            }
            return false;
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            ref var node = ref GetTrieNode(key, false);
            if (node.HasValue)
            {
                value = node.Value;
                node.ClearValue();
                return true;
            }
            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Save(TKey key, TValue value)
        {
            ref var node = ref GetTrieNode(key, true);
            if (node.IsNull)
            {
                throw new KeyNotFoundException($"{key} not found..");
            }
            if (!node.HasValue)
            {
                count++;
            }
            node.Update(key, value);
        }

        public int Update(Func<TKey, TValue, (bool replace, TValue value)> func)
        {
            return Update(func, default);
        }

        protected void EnsureCapacity(UInt32 i1)
        {
            if (this.Buffer.Length <= i1)
            {
                // add 16  more...
                var b = new TrieNode[i1 + grow];
                Array.Copy(this.Buffer, b, this.Buffer.Length);
                this.Buffer = b;
            }
        }


        internal struct TrieNode
        {

            internal static TrieNode Empty = new TrieNode
            {
                State = TrieNodeState.Null
            };

            public bool IsNull
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return this.State == TrieNodeState.Null;
                }
            }

            private TrieNodeState State;

            /// <summary>
            /// Index to next node set...
            /// </summary>
            public UInt32 FirstChildIndex;

            private TValue _Value;

            public void UpdateIndex(UInt32 index)
            {
                if (State == TrieNodeState.Null)
                    throw new InvalidOperationException();
                this.FirstChildIndex = index;
                this.State |= TrieNodeState.HasIndex;
            }

            public void Update(TKey key, TValue value)
            {
                if (State == TrieNodeState.Null)
                    throw new InvalidOperationException();
                this.Key = key;
                this.State |= TrieNodeState.HasValue;
                this._Value = value;
            }

            public void UpdateDefaultValue(TKey key, TValue value)
            {
                if (State == TrieNodeState.Null)
                    throw new InvalidOperationException();
                this.Key = key;
                this.State = (this.State & TrieNodeState.HasIndex) | TrieNodeState.HasDefaultValue;
                this._Value = value;
            }

            public TKey Key;

            public TValue Value
            {
                get => this._Value;
            }


            public bool HasIndex
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return (State & TrieNodeState.HasIndex) > 0;
                }
            }

            public bool HasValue
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return State != TrieNodeState.Null && (State & TrieNodeState.HasValue) > 0;
                }
            }

            public bool HasDefaultValue
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return State != TrieNodeState.Null && (State & TrieNodeState.HasDefaultValue) > 0;
                }
            }


            public void ClearValue()
            {
                this.State &= TrieNodeState.HasIndex;
            }

        }
    }
}
