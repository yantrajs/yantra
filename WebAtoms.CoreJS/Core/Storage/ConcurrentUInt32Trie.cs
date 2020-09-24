using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using WebAtoms.CoreJS.Core.Storage;

namespace WebAtoms.CoreJS.Core
{
    internal class ConcurrentBinaryByteMap<T> : IBitTrie<uint, T, BinaryByteMap<T>.TrieNode>
    {

        #region Struct TrieNode

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

            private T _Value;

            public void UpdateIndex(UInt32 index)
            {
                if (State == TrieNodeState.Null)
                    throw new InvalidOperationException();
                this.FirstChildIndex = index;
                this.State |= TrieNodeState.HasIndex;
            }

            public void Update(uint key, T value)
            {
                if (State == TrieNodeState.Null)
                    throw new InvalidOperationException();
                this.Key = key;
                this.State |= TrieNodeState.HasValue;
                this._Value = value;
            }

            public void UpdateDefaultValue(uint key, T value)
            {
                if (State == TrieNodeState.Null)
                    throw new InvalidOperationException();
                this.Key = key;
                this.State = (this.State & TrieNodeState.HasIndex) | TrieNodeState.HasDefaultValue;
                this._Value = value;
            }

            public uint Key;

            public T Value
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
        #endregion

        private ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();

        private UInt32 next = 4;
        private UInt32 size = 4;
        private UInt32 grow = 16;

        private TrieNode[] Buffer;

        public IEnumerable<(uint Key, T Value)> AllValues
        {
            get
            {
                lockSlim.EnterReadLock();
                foreach (var item in this.Enumerate(0))
                    yield return item;
                lockSlim.ExitReadLock();
            }
        }

        public T this[uint key]
        {
            get
            {
                ref var n = ref GetTrieNode(key);
                if (n.HasValue)
                    return n.Value;
                return default;
            }
            set => Save(key, value);
        }

        public ConcurrentBinaryByteMap()
        {
            Buffer = new TrieNode[grow];
        }

        public int Update(Func<UInt32, T, (bool replace, T value)> update)
        {
            try
            {
                lockSlim.EnterUpgradeableReadLock();
                return Update(update, 0);
            }
            finally
            {
                lockSlim.EnterUpgradeableReadLock();
            }
        }

        private int Update(Func<UInt32, T, (bool replace, T value)> update, UInt32 index)
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
                        lockSlim.EnterWriteLock();
                        Buffer[i].Update(node.Key, uv.value);
                        count++;
                        lockSlim.ExitWriteLock();
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

        protected IEnumerable<(uint key, T value)> Enumerate(UInt32 index)
        {
            var last = index + this.size;
            for (UInt32 i = index; i < last; i++)
            {
                var node = Buffer[i];
                if (node.HasValue)
                {
                    yield return (node.Key, node.Value);
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

        ref TrieNode GetTrieNode(uint key, bool create = false)
        {
            ref var node = ref TrieNode.Empty;

            // only case for zero...
            if (key == 0)
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
            for (i = 0; i <= last; i += 2)
            {
                byte bk = (byte)((key & start) >> i);
                if (index == uint.MaxValue)
                {
                    node = ref Buffer[bk];
                    index = bk;
                }
                else
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

        public void Save(uint key, T value)
        {
            lockSlim.EnterWriteLock();
            ref var node = ref GetTrieNode(key, true);
            node.Update(key, value);
            lockSlim.ExitWriteLock();
        }

        public T GetOrCreate(uint key, Func<T> value)
        {
            try
            {
                lockSlim.EnterUpgradeableReadLock();
                ref var node = ref GetTrieNode(key, true);
                if (node.HasValue)
                    return node.Value;
                var r = value();
                lockSlim.EnterWriteLock();
                node.Update(key, r);
                lockSlim.ExitWriteLock();
                return r;
            } finally
            {                
                lockSlim.ExitUpgradeableReadLock();
            }
        }

        public bool TryGetValue(uint key, out T value)
        {
            lockSlim.EnterReadLock();
            ref var node = ref GetTrieNode(key, false);
            if (node.HasValue)
            {
                value = node.Value;
                lockSlim.ExitReadLock();
                return true;
            }
            value = default;
            lockSlim.ExitReadLock();
            return false;
        }

        public bool RemoveAt(uint key)
        {
            lockSlim.EnterUpgradeableReadLock();
            ref var node = ref GetTrieNode(key, false);
            if (node.HasValue)
            {
                lockSlim.EnterWriteLock();
                node.ClearValue();
                lockSlim.ExitWriteLock();
                lockSlim.ExitUpgradeableReadLock();
                return true;
            }
            lockSlim.ExitUpgradeableReadLock();
            return false;
        }

        public bool HasKey(uint key)
        {
            lockSlim.EnterReadLock();
            ref var node = ref GetTrieNode(key, false);
            var b = node.HasValue;
            lockSlim.ExitReadLock();
            return b;
        }

        public bool TryRemove(uint key, out T value)
        {
            lockSlim.EnterUpgradeableReadLock();
            ref var node = ref GetTrieNode(key, false);
            if (node.HasValue)
            {
                value = node.Value;
                lockSlim.EnterWriteLock();
                node.ClearValue();
                lockSlim.ExitWriteLock();
                lockSlim.ExitUpgradeableReadLock();
                return true;
            }
            value = default;
            lockSlim.ExitUpgradeableReadLock();
            return false;
        }

    }
}
