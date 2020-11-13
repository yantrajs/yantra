using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using YantraJS.Core.Storage;

namespace YantraJS.Core
{
    //internal abstract class ConcurrentTrie<TKey, T> : IBitTrie<TKey, T, ConcurrentTrie<TKey, T>.TrieNode>
    //{

    //    #region Struct TrieNode

    //    internal struct TrieNode
    //    {

    //        internal static TrieNode Empty = new TrieNode
    //        {
    //            State = TrieNodeState.Null
    //        };

    //        public bool IsNull
    //        {
    //            [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //            get
    //            {
    //                return this.State == TrieNodeState.Null;
    //            }
    //        }

    //        private TrieNodeState State;

    //        /// <summary>
    //        /// Index to next node set...
    //        /// </summary>
    //        public UInt32 FirstChildIndex;

    //        private T _Value;

    //        public void UpdateIndex(UInt32 index)
    //        {
    //            if (State == TrieNodeState.Null)
    //                throw new InvalidOperationException();
    //            this.FirstChildIndex = index;
    //            this.State |= TrieNodeState.HasIndex;
    //        }

    //        public void Update(TKey key, T value)
    //        {
    //            if (State == TrieNodeState.Null)
    //                throw new InvalidOperationException();
    //            this.Key = key;
    //            this.State |= TrieNodeState.HasValue;
    //            this._Value = value;
    //        }

    //        public void UpdateDefaultValue(TKey key, T value)
    //        {
    //            if (State == TrieNodeState.Null)
    //                throw new InvalidOperationException();
    //            this.Key = key;
    //            this.State = (this.State & TrieNodeState.HasIndex) | TrieNodeState.HasDefaultValue;
    //            this._Value = value;
    //        }

    //        public TKey Key;

    //        public T Value
    //        {
    //            get => this._Value;
    //        }


    //        public bool HasIndex
    //        {
    //            [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //            get
    //            {
    //                return (State & TrieNodeState.HasIndex) > 0;
    //            }
    //        }

    //        public bool HasValue
    //        {
    //            [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //            get
    //            {
    //                return State != TrieNodeState.Null && (State & TrieNodeState.HasValue) > 0;
    //            }
    //        }

    //        public bool HasDefaultValue
    //        {
    //            [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //            get
    //            {
    //                return State != TrieNodeState.Null && (State & TrieNodeState.HasDefaultValue) > 0;
    //            }
    //        }


    //        public void ClearValue()
    //        {
    //            this.State &= TrieNodeState.HasIndex;
    //        }

    //    }
    //    #endregion

    //    private ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();

    //    protected UInt32 next;
    //    protected readonly UInt32 size;
    //    protected readonly uint grow;

    //    protected TrieNode[] Buffer;

    //    public long Count => throw new NotImplementedException();

    //    public IEnumerable<(TKey Key, T Value)> AllValues
    //    {
    //        get
    //        {
    //            lockSlim.EnterReadLock();
    //            var list = new List<(TKey Key, T Value)>(this.Buffer.Length / 4);
    //            Enumerate(0, list);
    //            lockSlim.ExitReadLock();
    //            return list;
    //        }
    //    }

    //    public T this[TKey key]
    //    {
    //        get
    //        {
    //            ref var n = ref GetTrieNode(key);
    //            if (n.HasValue)
    //                return n.Value;
    //            return default;
    //        }
    //        set => Save(key, value);
    //    }

    //    public ConcurrentTrie(uint size, uint grow)
    //    {
    //        this.size = size;
    //        this.next = size;
    //        this.grow = grow;
    //        Buffer = new TrieNode[grow];
    //    }

    //    public int Update(Func<TKey, T, (bool replace, T value)> update)
    //    {
    //        try
    //        {
    //            lockSlim.EnterUpgradeableReadLock();
    //            return Update(update, 0);
    //        }
    //        finally
    //        {
    //            lockSlim.EnterUpgradeableReadLock();
    //        }
    //    }

    //    protected int Update(Func<TKey, T, (bool replace, T value)> update, UInt32 index)
    //    {
    //        int count = 0;
    //        var last = index + this.size;
    //        for (uint i = index; i < last; i++)
    //        {
    //            var node = Buffer[i];
    //            var fi = node.FirstChildIndex;
    //            if (node.HasValue)
    //            {
    //                var uv = update(node.Key, node.Value);
    //                if (uv.replace)
    //                {
    //                    lockSlim.EnterWriteLock();
    //                    Buffer[i].Update(node.Key, uv.value);
    //                    count++;
    //                    lockSlim.ExitWriteLock();
    //                    continue;
    //                }
    //                continue;
    //            }
    //            if (!node.HasIndex)
    //            {
    //                continue;
    //            }
    //            count += Update(update, fi);
    //        }
    //        return count;
    //    }

    //    protected void Enumerate(UInt32 index, List<(TKey Key, T Value)> all)
    //    {
    //        var last = index + this.size;
    //        for (UInt32 i = index; i < last; i++)
    //        {
    //            ref var node = ref Buffer[i];
    //            if (node.HasValue)
    //            {
    //                all.Add((node.Key, node.Value));
    //            }
    //        }
    //        for (UInt32 i = index; i < last; i++)
    //        {
    //            ref var node = ref Buffer[i];
    //            if (!node.HasIndex)
    //            {
    //                continue;
    //            }
    //            var fi = node.FirstChildIndex;
    //            Enumerate(fi, all);
    //        }
    //    }

    //    protected abstract ref TrieNode GetTrieNode(TKey key, bool create = false);

    //    protected void EnsureCapacity(UInt32 i1)
    //    {
    //        if (this.Buffer.Length <= i1)
    //        {
    //            // add 16  more...
    //            var b = new TrieNode[i1 + grow];
    //            Array.Copy(this.Buffer, b, this.Buffer.Length);
    //            this.Buffer = b;
    //        }
    //    }

    //    public void Save(TKey key, T value)
    //    {
    //        lockSlim.EnterWriteLock();
    //        ref var node = ref GetTrieNode(key, true);
    //        node.Update(key, value);
    //        lockSlim.ExitWriteLock();
    //    }

    //    public T GetOrCreate(TKey key, Func<T> value)
    //    {
    //        try
    //        {
    //            lockSlim.EnterUpgradeableReadLock();
    //            ref var node = ref GetTrieNode(key, true);
    //            if (node.HasValue)
    //                return node.Value;
    //            var r = value();
    //            lockSlim.EnterWriteLock();
    //            node.Update(key, r);
    //            lockSlim.ExitWriteLock();
    //            return r;
    //        }
    //        finally
    //        {
    //            lockSlim.ExitUpgradeableReadLock();
    //        }
    //    }

    //    public bool TryGetValue(TKey key, out T value)
    //    {
    //        lockSlim.EnterReadLock();
    //        ref var node = ref GetTrieNode(key, false);
    //        if (node.HasValue)
    //        {
    //            value = node.Value;
    //            lockSlim.ExitReadLock();
    //            return true;
    //        }
    //        value = default;
    //        lockSlim.ExitReadLock();
    //        return false;
    //    }

    //    public bool RemoveAt(TKey key)
    //    {
    //        lockSlim.EnterUpgradeableReadLock();
    //        ref var node = ref GetTrieNode(key, false);
    //        if (node.HasValue)
    //        {
    //            lockSlim.EnterWriteLock();
    //            node.ClearValue();
    //            lockSlim.ExitWriteLock();
    //            lockSlim.ExitUpgradeableReadLock();
    //            return true;
    //        }
    //        lockSlim.ExitUpgradeableReadLock();
    //        return false;
    //    }

    //    public bool HasKey(TKey key)
    //    {
    //        lockSlim.EnterReadLock();
    //        ref var node = ref GetTrieNode(key, false);
    //        var b = node.HasValue;
    //        lockSlim.ExitReadLock();
    //        return b;
    //    }

    //    public bool TryRemove(TKey key, out T value)
    //    {
    //        lockSlim.EnterUpgradeableReadLock();
    //        ref var node = ref GetTrieNode(key, false);
    //        if (node.HasValue)
    //        {
    //            value = node.Value;
    //            lockSlim.EnterWriteLock();
    //            node.ClearValue();
    //            lockSlim.ExitWriteLock();
    //            lockSlim.ExitUpgradeableReadLock();
    //            return true;
    //        }
    //        value = default;
    //        lockSlim.ExitUpgradeableReadLock();
    //        return false;
    //    }

    //}
}
