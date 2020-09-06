using Esprima.Ast;
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

        protected abstract IEnumerable<(TKey key, TValue value, UInt32 index)> Enumerate(UInt32 index);

        public abstract int Update(Func<TKey, TValue, (bool replace, TValue value)> update, UInt32 start = 0);

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

        public bool RemoveAt(TKey key)
        {
            ref var node = ref GetTrieNode(key, false);
            if (node.HasValue)
            {
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
            node.Update(key, value);
        }


        internal struct TrieNode
        {
            private const UInt32 HasValueFlag = 0x1;
            private const UInt32 HasIndexFlag = 0x10;

            private const UInt32 EmptyFlag = 0xFFFFFFFF;


            internal static TrieNode Empty = new TrieNode
            {
                State = 0xFFFFFFFF
            };

            public bool IsNull
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return this.State == EmptyFlag;
                }
            }

            private UInt32 State;

            /// <summary>
            /// Index to next node set...
            /// </summary>
            public UInt32 FirstChildIndex;

            private TValue _Value;

            public void UpdateIndex(UInt32 index)
            {
                if (State == EmptyFlag)
                    throw new InvalidOperationException();
                this.FirstChildIndex = index;
                this.State |= HasIndexFlag;
            }

            public void Update(TKey key, TValue value)
            {
                if (State == EmptyFlag)
                    throw new InvalidOperationException();
                this.Key = key;
                this.State |= HasValueFlag;
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
                    return (State & HasIndexFlag) > 0;
                }
            }

            public bool HasValue
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return State !=  EmptyFlag && (State & HasValueFlag) > 0;
                }
            }

            public void ClearValue()
            {
                this.State &= 0x10;
            }

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

    internal class BinaryByteMap<T>: BaseMap<UInt32, T>
    {

        private UInt32 next = 4;
        private UInt32 size = 4;
        private UInt32 grow = 16;

        protected BinaryByteMap()
        {
            Buffer = new TrieNode[grow];
        }

        public override int Update(Func<UInt32, T, (bool replace,T value)> update, UInt32 index = 0)
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
                        Buffer[i].Update(node.Key, uv.value);
                        count++;
                        continue;
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

        protected override IEnumerable<(uint key, T value, UInt32 index)> Enumerate(UInt32 index)
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

        protected override ref TrieNode GetTrieNode(UInt32 key, bool create = false)
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

        void EnsureCapacity(UInt32 i1)
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
