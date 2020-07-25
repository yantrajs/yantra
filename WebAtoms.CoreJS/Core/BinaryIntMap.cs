using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    internal abstract class BaseMap<TKey, TValue>
    {
        protected TrieNode[] Buffer;

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

        public IEnumerable<(TKey Key, TValue Value)> AllValues()
        {
            foreach (var a in Enumerate(0))
            {
                yield return (a.key, a.value);
            }
        }

        protected abstract IEnumerable<(TKey key, TValue value, uint index)> Enumerate(uint index);

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
                    Buffer[i].Value = null;
                    return true;
                }
            }
            return false;
        }

        public TValue GetOrCreate(TKey key, Func<TValue> factory)
        {
            ref var node = ref GetTrieNode(key, true);
            if (node.Value != null)
            {
                return node.Value.Value;
            }
            var v = factory();
            node.Value = new NodeValue { Key = key, Value = v };
            return v;
        }

        protected abstract ref TrieNode GetTrieNode(TKey key, bool create = false);

        public bool TryGetValue(TKey key, out TValue value)
        {
            ref var node = ref GetTrieNode(key);
            if (node.Value != null)
            {
                value = node.Value.Value;
                return true;
            }
            value = default;
            return false;
        }

        public bool RemoveAt(TKey key)
        {
            ref var node = ref GetTrieNode(key, false);
            if (node.Value != null)
            {
                node.Value = null;
                return true;
            }
            return false;
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            ref var node = ref GetTrieNode(key, false);
            if (node.Value != null)
            {
                value = node.Value.Value;
                node.Value = null;
                return true;
            }
            value = default;
            return false;
        }

        public void Save(TKey key, TValue value)
        {
            ref var node = ref GetTrieNode(key, true);
            node.Value = new NodeValue { Key = key, Value = value };
        }


        internal class NodeValue
        {
            public TKey Key;
            public TValue Value;
        }

        internal struct TrieNode
        {

            internal static TrieNode Empty = new TrieNode
            {
                FirstChildIndex = uint.MaxValue
            };

            internal bool IsEmpty => FirstChildIndex == 0;

            /// <summary>
            /// Index to next node set...
            /// </summary>
            internal UInt32 FirstChildIndex;

            /// <summary>
            /// Value of the current node
            /// </summary>
            internal NodeValue Value;
        }
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

        private uint next = 4;

        private uint grow = 32;

        protected BinaryByteMap()
        {
            Buffer = new TrieNode[grow];
        }

        protected override IEnumerable<(uint key, T value, uint index)> Enumerate(uint index)
        {
            var last = index + 4;
            for (uint i = index; i < last; i++)
            {
                var node = Buffer[i];
                var fi = node.FirstChildIndex;
                var v = node.Value;
                if (v != null)
                {
                    yield return (v.Key, v.Value, i);
                }
                if (fi == 0)
                {
                    continue;
                }
                foreach (var a in Enumerate(fi)) yield return a;
            }
        }

        protected override ref TrieNode GetTrieNode(uint key, bool create = false)
        {
            ref var node = ref TrieNode.Empty;
            uint start = (uint)((uint)0x3 << (int)30);
            uint index = 0;
            // incremenet of two bits...
            for (int i = 30; i >= 0; i-=2)
            {
                byte bk = (byte)((key & start) >> i);
                index += bk;
                index = GetNode(index, create);
                if (index == uint.MaxValue)
                {
                    return ref node;
                }
                start = start >> 2;
            }

            return ref Buffer[index];
        }

        private uint GetNode(uint position, bool create = false)
        {
            if (position >= Buffer.Length)
            {
                if (!create)
                {
                    return uint.MaxValue;
                }
                this.EnsureCapacity(position);
            }
            ref var v = ref Buffer[position];
            if (v.FirstChildIndex == 0)
            {
                if (!create)
                {
                    return uint.MaxValue;
                }
                v.FirstChildIndex = next;
                next += 4;
                this.EnsureCapacity(v.FirstChildIndex);
            }
            return v.FirstChildIndex;
            // return ref Buffer[v.FirstChildIndex];
            // return v.FirstChildIndex;
        }

        void EnsureCapacity(uint i1)
        {
            if (this.Buffer.Length <= i1)
            {
                // add 16  more...
                var b = new TrieNode[i1 + grow];
                Array.Copy(this.Buffer, b, this.Buffer.Length);
                this.Buffer = b;
            }
        }
    }

}
