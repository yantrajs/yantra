using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using WebAtoms.CoreJS.Core.Storage;

namespace WebAtoms.CoreJS.Core
{
    internal abstract class BaseMap<TKey, TValue>: IBitTrie<TKey, TValue, BaseMap<TKey,TValue>.TrieNode>
        where TKey: IComparable<TKey>
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

        public IEnumerable<(TKey Key, TValue Value)> AllValues
        {
            get {
                foreach (var (k,v, _) in Enumerate(0))
                {
                    yield return (k, v);
                }
            }
        }

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
            foreach(var (k, v, _) in Enumerate(0))
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
            foreach(var (_,v,i) in Enumerate(0))
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
                    return (State &  TrieNodeState.HasIndex) > 0;
                }
            }

            public bool HasValue
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return State !=  TrieNodeState.Null && (State & TrieNodeState.HasValue) > 0;
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

    internal enum TrieNodeState : byte
    {
        HasValue = 1,
        HasIndex = 2,
        HasDefaultValue = 4,
        Null = 0xff
    }

    internal static class ByteEnumerableExtensions {

        public static IEnumerable<byte> ToBytes(this UInt32 input)
        {
            yield return (byte)((input & (UInt32)0xFF000000) >> 24);
            yield return (byte)((input & (UInt32)0xFF0000) >> 16);
            yield return (byte)((input & (UInt32)0xFF00) >> 8);
            yield return (byte)((input & (UInt32)0xFF));
        }

    }

    internal class BinaryUInt32Map<T>: BinaryByteMap<T>
    {
        //public T this [UInt32 index]
        //{
        //    get
        //    {
        //        if (this.TryGetValue(index.ToBytes(), out var t))
        //            return t;
        //        return default;
        //    }
        //    set
        //    {
        //        this.Save(index.ToBytes(), value);
        //    }
        //}
    }

    internal class BinaryByteMap<T>: BaseMap<uint, T>
    {

        public BinaryByteMap(): base(4, 16)
        {
        }


        protected override IEnumerable<(uint Key, T Value, uint index)> Enumerate(UInt32 index)
        {
            var last = index + this.size;
            for (UInt32 i = index; i < last; i++)
            {
                var node = Buffer[i];
                if (node.HasValue)
                {
                    yield return (node.Key, node.Value, i);
                }
            }
            for (UInt32 i = index; i < last; i++)
            {
                var node = Buffer[i];
                if (!node.HasIndex)
                {
                    continue;
                }
                var fi = node.FirstChildIndex;
                foreach (var a in Enumerate(fi)) yield return a;
            }
        }

        protected override ref TrieNode GetTrieNode(uint key, bool create = false)
        {
            ref var node = ref TrieNode.Empty;

            // only case for zero...
            if(key == 0)
            {
                return ref Buffer[0];
            }

            UInt32 start = 0xc0000000;
            Int32 i;
            for (i = 30; i >= 0; i -= 2)
            {
                byte bk = (byte)((key & start) >> i);
                if (bk == 0)
                {
                    start = start >> 2;
                    continue;
                }
                break;
            }
            var last = i;
            start = 0x3;
            uint index = uint.MaxValue;
            // incremenet of two bits...
            for (i = 0; i <= last; i+=2)
            {
                byte bk = (byte)((key & start) >> i);
                if (index == uint.MaxValue)
                {
                    node = ref Buffer[bk];
                    index = bk;
                } else
                {
                    if (!node.HasIndex)
                    {
                        if (!create)
                        {
                            return ref TrieNode.Empty;
                        }

                        var position = next;
                        next += this.size;
                        this.EnsureCapacity(next);
                        Buffer[index].UpdateIndex(position);
                        index = position + bk;
                        node = ref Buffer[index];
                    }
                    else
                    {
                        index = node.FirstChildIndex + bk;
                        node = ref Buffer[index];
                    }
                }
                start = start << 2;
            }

            return ref node;
        }

    }
}
