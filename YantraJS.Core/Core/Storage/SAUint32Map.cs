using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.FastParser;
using YantraJS.Core.Storage;

namespace YantraJS.Core
{

    public struct SAUint32Map<T>
    {
        public const int NodeBlock = 4;

        [DebuggerDisplay("{Key}: {Value}")]
        public struct KeyValue
        {
            public uint Key;
            public T Value;
        }


        enum NodeState : byte
        {
            Empty = 0,
            Filled = 1,
            HasValue = 4
        }

        static Node Empty = new Node();

        [DebuggerDisplay("{Key}={Value}")]
        struct Node
        {
            public bool HasValue
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return (State & NodeState.HasValue) > 0;
                }
            }

            /// <summary>
            /// Current key
            /// </summary>
            public uint Key;
            public NodeState State;
            /// <summary>
            /// Current value
            /// </summary>
            public T Value;
            /// <summary>
            /// Index of First Child.
            /// All children must be allocated
            /// in advance.
            /// </summary>
            public VirtualArray Children;
        }

        // private Node[] storage;
        // private uint last;

        private VirtualMemory<Node> nodes;

        // first set of roots
        private VirtualArray roots;

        public T this[uint index]
        {
            get
            {
                ref var node = ref GetNode(index);
                return node.HasValue ? node.Value : default;
            }
        }

        // public bool HasChildren => storage != null;

        public bool IsNull => nodes.IsEmpty;


        public IEnumerable<KeyValue> All
        {
            get
            {
                foreach (var (k, v) in AllValues())
                    yield return new KeyValue { Key = k, Value = v };
            }
        }

        public IEnumerable<(uint Key, T Value)> AllValues()
        {
            if (!this.nodes.IsEmpty)
            {
                //if (!this.roots.IsEmpty)
                //{
                //    foreach(var c in this.EnumerateNode(this.roots))
                //    {
                //        yield return c;
                //    }
                //}
                for (int i = 0; i < this.nodes.Count; i++)
                {
                    var node = this.nodes.GetAt(i);
                    if (node.HasValue)
                    {
                        yield return (node.Key, node.Value);
                    }
                }
            }
        }

        //private IEnumerable<(uint Key, T Value)> EnumerateNode(VirtualArray nodes)
        //{
        //    for (var i = 0; i<nodes.Length;i++)
        //    {
        //        var n = this.nodes[nodes, i];
        //        if (n.HasValue)
        //        {
        //            yield return (n.Key, n.Value);
        //        }
        //    }
        //    for (var i = 0; i < nodes.Length; i++)
        //    {
        //        var n = this.nodes[nodes, i];
        //        if (!n.Children.IsEmpty)
        //        {
        //            foreach (var c in this.EnumerateNode(n.Children))
        //            {
        //                yield return c;
        //            }
        //        }
        //    }
        //}

        public bool HasKey(uint key)
        {
            ref var node = ref GetNode(key);
            return node.HasValue;
        }

        public bool TryGetValue(uint key, out T value)
        {
            ref var node = ref GetNode(key);
            if (node.HasValue)
            {
                value = node.Value;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryRemove(uint key, out T value)
        {
            ref var node = ref GetNode(key);
            if (node.HasValue)
            {
                value = node.Value;
                node.Value = default;
                node.State = NodeState.Filled;
                return true;
            }
            value = default;
            return false;
        }

        public void Save(uint key, T value)
        {
            ref var node = ref GetNode(key, true);
            node.Value = value;
            node.State |= NodeState.HasValue;
        }

        public ref T Put(uint key)
        {
            ref var node = ref GetNode(key, true);
            node.State |= NodeState.HasValue;
            return ref node.Value;
        }

        public ref T GetRefOrDefault(uint key, ref T def) {
            ref var node = ref GetNode(key);
            if (node.HasValue)
            {
                return ref node.Value;
            }
            return ref def;
        }

        public bool RemoveAt(uint key)
        {
            ref var node = ref GetNode(key);
            if (node.HasValue)
            {
                node.State = NodeState.Filled;
                node.Value = default;
                return true;
            }
            return false;
        }


        private ref Node GetNode(uint originalKey, bool create = false)
        {
            ref var node = ref Empty;

            if (this.roots.IsEmpty) { 
                if (!create) {
                    return ref Empty;
                }
                // extend...
                this.roots = this.nodes.Allocate(4);
                this.nodes[this.roots, 0].State = NodeState.Filled;
            }

            if (originalKey == 0)
            {
                node = ref this.nodes[this.roots, 0];
                return ref node;
            }

            var leaves = this.roots;

            // let us walk the nodes...
            for (long key = originalKey; key > 0; key >>= 2)
            {
                var index = (int)(key & 0x3);
                node = ref this.nodes[leaves, index];
                if (node.Key == originalKey) {
                    if (create)
                    {
                        if (node.State == NodeState.Empty)
                        {
                            node.State = NodeState.Filled;
                        }
                    }
                    return ref node;
                }
                if (create)
                {
                    if (node.State == NodeState.Empty)
                    {
                        // lets occupy current node.
                        node.State = NodeState.Filled;
                        node.Key = originalKey;
                        return ref node;
                    }
                    if (node.Key > originalKey)
                    {
                        // need to make this non recursive...
                        var oldKey = node.Key;
                        var oldValue = node.Value;
                        // var oldChild = node.Children;
                        node.Key = originalKey;
                        node.State = NodeState.Filled;
                        node.Value = default;
                        ref var newChild = ref GetNode(oldKey, true);
                        newChild.Key = oldKey;
                        newChild.Value = oldValue;
                        // var newChildren = newChild.Children;
                        // newChild.Children = oldChild;
                        newChild.State |= NodeState.HasValue;
                        // this is case when array is resized
                        // and we still might have reference to old node
                        node = ref this.nodes[leaves, index];
                        // node.Children = newChildren;
                        return ref node;
                    }
                    node.State |= NodeState.Filled;
                    if (node.Children.IsEmpty)
                    {
                        var c = this.nodes.Allocate(4);
                        // allocation may have moved node
                        node = ref this.nodes[leaves, index];
                        node.Children = c;
                    }
                }
                var next = node.Children;
                if (next.IsEmpty)
                {
                    return ref Empty;
                }
                leaves = next;
            }
            if (node.Key == originalKey)
            {
                return ref node;
            }
            return ref Empty;
        }

        internal void Resize(int size)
        {
            if (size < 0)
            {
                return;
            }
            // right align to 4 bits..
            size = ((size / 4)+1)*4;
            this.nodes.SetCapacity(size);
        }
    }


    //internal abstract class BaseMap<TKey, TValue> : IBitTrie<TKey, TValue, BaseMap<TKey, TValue>.TrieNode>
    //    where TKey : IComparable<TKey>
    //{

    //    protected static uint ToSize(int r, uint blocks)
    //    {
    //        var ri = (uint)r;
    //        return ((ri / blocks)+ 1)*blocks;
    //    }

    //    protected TrieNode[] Buffer;

    //    private long count;

    //    protected uint size;
    //    protected uint next;
    //    protected readonly uint grow;

    //    public long Count => count;

    //    public BaseMap(uint size, uint grow)
    //    {
    //        this.size = size;
    //        this.next = size;
    //        this.grow = grow;
    //        Buffer = new TrieNode[grow];
    //    }


    //    public TValue this[TKey input]
    //    {
    //        get
    //        {
    //            if (this.TryGetValue(input, out var t))
    //                return t;
    //            return default;
    //        }
    //        set
    //        {
    //            this.Save(input, value);
    //        }
    //    }

    //    /// <summary>
    //    /// Recursive enumerators are bad...
    //    /// </summary>
    //    public IEnumerable<(TKey Key, TValue Value)> AllValues
    //    {
    //        get
    //        {
    //           List<(TKey key, TValue value)> all = new List<(TKey key, TValue value)>(this.Buffer.Length/4);
    //            //void Yield(uint start, uint size, TrieNode[] buffer, List<(TKey, TValue)> list)
    //            //{
    //            //    var last = start + size;
    //            //    for (uint i = start; i < last; i++)
    //            //    {
    //            //        ref var node = ref buffer[i];
    //            //        if (node.HasValue)
    //            //        {
    //            //            list.Add((node.Key, node.Value));
    //            //        }
    //            //        if (node.HasIndex)
    //            //        {
    //            //            Yield(node.FirstChildIndex, size, buffer, list);
    //            //        }
    //            //    }
    //            //}
    //            //Yield(0, this.size, Buffer, all);
    //            Enumerate(0, all);
    //            return all;
    //        }
    //    }


    //    ////ref struct Enumerator 
    //    ////{
    //    ////    private BaseMap<TKey, TValue> map;
    //    ////    private uint index;
    //    ////    private TValue value;
    //    ////    public Enumerator(BaseMap<TKey,TValue> map, uint start)
    //    ////    {
    //    ////        this.map = map;
    //    ////        index = start;
    //    ////        value = default;
    //    ////    }

    //    ////    public bool MoveNext()
    //    ////    {
    //    ////        if (index < this.map.size)
    //    ////        {
    //    ////            ref var node = ref map.Buffer[index];
    //    ////            if (node.HasValue)
    //    ////            {
    //    ////                value = node.Value;
    //    ////                return true;
    //    ////            }
    //    ////        } else
    //    ////        {

    //    ////        }
    //    ////        index++;
    //    ////        return false;
    //    ////    }

    //    ////    public TValue Current => value;

    //    ////}

    //    protected abstract void Enumerate(UInt32 index, List<(TKey Key,TValue Value)> all);

    //    protected int Update(Func<TKey, TValue, (bool replace, TValue value)> update, UInt32 index = 0)
    //    {
    //        int count = 0;
    //        var last = index + this.size;
    //        for (uint i = index; i < last; i++)
    //        {
    //            ref var node = ref Buffer[i];
    //            var fi = node.FirstChildIndex;
    //            if (node.HasValue)
    //            {
    //                var (replace, value) = update(node.Key, node.Value);
    //                if (replace)
    //                {
    //                    node.Update(node.Key, value);
    //                    count++;
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

    //    public bool TryGetKeyOf(TValue value, out TKey key)
    //    {
    //        foreach (var (k, v) in this.AllValues)
    //        {
    //            if (v.Equals(value))
    //            {
    //                key = k;
    //                return true;
    //            }
    //        }
    //        key = default;
    //        return false;
    //    }

    //    public bool RemoveValue(TValue value)
    //    {
    //        bool removed = false;
    //        void Yield(uint start)
    //        {
    //            var last = start + this.size;
    //            for (uint i = start; i < last; i++)
    //            {
    //                ref var node = ref Buffer[i];
    //                if (node.HasValue)
    //                {
    //                    if (node.Value.Equals(value))
    //                    {
    //                        removed = true;
    //                        count--;
    //                        node.ClearValue();
    //                        return;
    //                    }
    //                }
    //                if (node.HasIndex)
    //                {
    //                    Yield(node.FirstChildIndex);
    //                }
    //            }
    //        }
    //        Yield(0);
    //        return removed;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public TValue GetOrCreate(TKey key, Func<TValue> factory)
    //    {
    //        ref var node = ref GetTrieNode(key, true);
    //        if (node.HasValue)
    //        {
    //            return node.Value;
    //        }
    //        var v = factory();
    //        node.Update(key, v);
    //        return v;
    //    }

    //    protected abstract ref TrieNode GetTrieNode(TKey key, bool create = false);


    //    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    //public ref TValue GetValue(TKey key, ref TValue def)
    //    //{
    //    //    ref var node = ref GetTrieNode(key);
    //    //    if (node.HasValue)
    //    //    {
    //    //        return ref node.Value;
    //    //        return true;
    //    //    }
    //    //    value = default;
    //    //    return false;
    //    //}

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool TryGetValue(TKey key, out TValue value)
    //    {
    //        ref var node = ref GetTrieNode(key);
    //        if (node.HasValue)
    //        {
    //            value = node.Value;
    //            return true;
    //        }
    //        value = default;
    //        return false;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool HasKey(TKey key)
    //    {
    //        ref var node = ref GetTrieNode(key);
    //        if (node.HasValue)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }


    //    public bool RemoveAt(TKey key)
    //    {
    //        ref var node = ref GetTrieNode(key, false);
    //        if (node.HasValue)
    //        {
    //            count--;
    //            node.ClearValue();
    //            return true;
    //        }
    //        return false;
    //    }

    //    public bool TryRemove(TKey key, out TValue value)
    //    {
    //        ref var node = ref GetTrieNode(key, false);
    //        if (node.HasValue)
    //        {
    //            value = node.Value;
    //            node.ClearValue();
    //            return true;
    //        }
    //        value = default;
    //        return false;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void Save(TKey key, TValue value)
    //    {
    //        ref var node = ref GetTrieNode(key, true);
    //        if (node.IsNull)
    //        {
    //            throw new KeyNotFoundException($"{key} not found..");
    //        }
    //        if (!node.HasValue)
    //        {
    //            count++;
    //        }
    //        node.Update(key, value);
    //    }

    //    public int Update(Func<TKey, TValue, (bool replace, TValue value)> func)
    //    {
    //        return Update(func, default);
    //    }

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

    //        private TValue _Value;

    //        public void UpdateIndex(UInt32 index)
    //        {
    //            if (State == TrieNodeState.Null)
    //                throw new InvalidOperationException();
    //            this.FirstChildIndex = index;
    //            this.State |= TrieNodeState.HasIndex;
    //        }

    //        public void Update(TKey key, TValue value)
    //        {
    //            if (State == TrieNodeState.Null)
    //                throw new InvalidOperationException();
    //            this.Key = key;
    //            this.State |= TrieNodeState.HasValue;
    //            this._Value = value;
    //        }

    //        public void UpdateDefaultValue(TKey key, TValue value)
    //        {
    //            if (State == TrieNodeState.Null)
    //                throw new InvalidOperationException();
    //            this.Key = key;
    //            this.State = (this.State & TrieNodeState.HasIndex) | TrieNodeState.HasDefaultValue;
    //            this._Value = value;
    //        }

    //        public TKey Key;

    //        public TValue Value
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
    //            this._Value = default;
    //        }

    //    }
    //}
}
