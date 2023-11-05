using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace YantraJS.Core.Core.Storage
{
    /// <summary>
    /// Mapping of uint to uint
    /// </summary>
    public struct StringMap<T>
    {
        private const int Bits = 2;
        private const int Size = 1 << Bits;
        private const int Mask = ~(-1 << Bits);


        enum NodeState : byte
        {
            Empty = 0,
            Filled = 1,
            HasValue = 4
        }

        static Node Empty = new Node();

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
            public HashedString Key;
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
            public uint Children;
        }

        private Node[] storage;
        private uint last;

        public bool IsNull => storage == null;

        public IEnumerable<(StringSpan Key, T Value)> AllValues()
        {
            if (storage != null)
            {
                for (int i = 0; i < storage.Length; i++)
                {
                    var node = storage[i];
                    if (node.HasValue)
                    {
                        yield return (node.Key.Value, node.Value);
                    }
                }
            } 
        }


        public T this[in HashedString index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref var node = ref GetNode(index);
                if (node.HasValue)
                    return node.Value;
                return default;
            }
            //[Obsolete("Use Put")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                ref var node = ref GetNode(index, true);
                node.State |= NodeState.HasValue;
                node.Value = value;

            }
        }

        public ref T Put(in HashedString index)
        {
            ref var node = ref GetNode(index, true);
            node.State |= NodeState.HasValue;
            return ref node.Value;
        }


        public T this[in StringSpan index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref var node = ref GetNode(index);
                if (node.HasValue)
                    return node.Value;
                return default;
            }
            [Obsolete("Use Put")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                ref var node = ref GetNode(index, true);
                node.State |= NodeState.HasValue;
                node.Value = value;

            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(in StringSpan key, out T value)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(in HashedString key, out T value)
        {
            ref var node = ref GetNode(in key);
            if (node.HasValue)
            {
                value = node.Value;
                return true;
            }
            value = default;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKey(in StringSpan key)
        {
            ref var node = ref GetNode(key);
            return node.HasValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemove(in StringSpan key, out T value)
        {
            // HashedString hsName = key;
            ref var node = ref GetNode(key);
            if (node.HasValue)
            {
                value = node.Value;
                node.State = NodeState.Filled;
                return true;
            }
            value = default;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Save(in HashedString key, T value)
        {
            ref var node = ref GetNode(in key, true);
            node.State |= NodeState.HasValue;
            node.Value = value;
        }

        private ref Node GetNode(in HashedString originalKey, bool create = false)
        {
            ref var node = ref Empty;

            if (storage == null)
            {
                if (!create)
                {
                    return ref node;
                }
                // extend...
                storage = new Node[16];
                ref var first = ref storage[0];
                first.State = NodeState.Filled;
                first.Key = "";
                last = Size;
            }

            if (originalKey.Value.IsEmpty)
            {
                node = ref storage[0];
                return ref node;
            }

            // let us walk the nodes...
            uint offset = 0;
            int start = originalKey.Hash;
            for(; start != 0; start >>= Bits)
            {
                var index = offset + (start & Mask);
                node = ref storage[index];
                if (node.Key == originalKey)
                {
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
                    if (node.Key.CompareToRef(in originalKey) > 0)
                    {
                        var oldKey = node.Key;
                        var oldValue = node.Value;
                        var oldChild = node.Children;
                        node.Key = originalKey;
                        node.State = NodeState.Filled;
                        node.Value = default;
                        ref var newChild = ref GetNode(oldKey, true);
                        newChild.Key = oldKey;
                        newChild.Value = oldValue;
                        newChild.State |= NodeState.HasValue;
                        // this is case when array is resized
                        // and we still might have reference to old node
                        node = ref storage[index];
                        return ref node;
                    }
                    node.State |= NodeState.Filled;
                    if (node.Children == 0)
                    {
                        node.Children = last;
                        last += Size;
                        if (last >= storage.Length)
                        {
                            global::System.Array.Resize(ref storage, storage.Length * 2);
                        }
                    }
                }
                var next = node.Children;
                if (next == 0)
                {
                    return ref Empty;
                }
                offset = next;
            }
            if (node.Key == originalKey)
            {
                return ref node;
            }
            var en = originalKey.Value.GetEnumerator();
            while (en.MoveNext(out var ch))
            {
                Int32 uch = ch;
                for (; uch > 0; uch >>= Bits)
                {
                    var index = start + uch & Mask;
                    node = ref storage[index];
                    if (node.Key == originalKey)
                    {
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
                        if (node.Key.CompareToRef(in originalKey) > 0)
                        {
                            var oldKey = node.Key;
                            var oldValue = node.Value;
                            var oldChild = node.Children;
                            node.Key = originalKey;
                            node.State = NodeState.Filled;
                            node.Value = default;
                            ref var newChild = ref GetNode(oldKey, true);
                            newChild.Key = oldKey;
                            newChild.Value = oldValue;
                            newChild.State |= NodeState.HasValue;
                            // this is case when array is resized
                            // and we still might have reference to old node
                            node = ref storage[index];
                            return ref node;
                        }
                        node.State |= NodeState.Filled;
                        if (node.Children == 0)
                        {
                            node.Children = last;
                            last += Size;
                            if (last >= storage.Length)
                            {
                                global::System.Array.Resize(ref storage, storage.Length + 16);
                            }
                        }
                    }
                    var next = node.Children;
                    if (next == 0)
                    {
                        return ref Empty;
                    }
                    offset = next;
                }
            }
            return ref Empty;
        }

        //private ref StringMap<T> GetNode(in HashedString originalKey, bool create = false)
        //{
        //    ref var node = ref Null;
        //    ref var current = ref this;

        //    if (originalKey.Hash == 0 && originalKey.Value.Length == 0)
        //    {
        //        // empty string....
        //        if (Nodes == null && !create)
        //        {
        //            return ref Null;
        //        }
        //        Nodes = Nodes ?? new StringMap<T>[Size];
        //        node = ref Nodes[0];
        //        if (node.Key.CompareToRef(in originalKey) != 0)
        //        {
        //            // move...
        //            var oldKey = node.Key;
        //            var oldValue = node.value;
        //            var oldState = node.State;
        //            node.Key = originalKey;
        //            node.State = MapValueState.HasDefaultValue | MapValueState.Filled;
        //            node.value = default;

        //            ref var child = ref GetNode(oldKey, true);
        //            child.Key = oldKey;
        //            child.value = oldValue;
        //            child.State = oldState;
        //        }
        //        node.State |= MapValueState.Filled;
        //        return ref node;

        //    }

        //    int start = originalKey.Hash;
        //    for(; start != 0; start >>= Bits)
        //    {
        //        if(current.Nodes == null)
        //        {
        //            if (!create)
        //                return ref Null;
        //            current.Nodes = new StringMap<T>[Size];
        //        }
        //        node = ref current.Nodes[start & Mask];
        //        if (!node.HasIndex)
        //        {
        //            if (create)
        //            {
        //                node.Key = originalKey;
        //                node.State = MapValueState.Filled;
        //                return ref node;
        //            }
        //            return ref Null;
        //        }
        //        if (node.Key == originalKey)
        //        {
        //            return ref node;
        //        }
        //        if (create) {
        //            if (node.Key.CompareToRef(in originalKey) > 0)
        //            {
        //                node.State = MapValueState.HasDefaultValue | MapValueState.Filled;
        //                var oldKey = node.Key;
        //                var oldValue = node.value;
        //                node.Key = originalKey;
        //                ref var child = ref this.GetNode(in oldKey, true);
        //                child.Key = oldKey;
        //                child.State = MapValueState.HasValue | MapValueState.Filled;
        //                child.value = oldValue;
        //                return ref node;
        //            }
        //        }
        //        current = ref node;
        //    }
        //    if (node.Key == originalKey)
        //    {
        //        return ref node;
        //    }
        //    var en = originalKey.Value.GetEnumerator();
        //    while(en.MoveNext(out var ch))
        //    {
        //        Int32 uch = ch;
        //        for (; uch > 0; uch >>= Bits)
        //        {
        //            if(current.Nodes == null)
        //            {
        //                if (!create)
        //                    return ref Null;
        //                current.Nodes = new StringMap<T>[Size];
        //            }

        //            node = ref current.Nodes[uch & Mask];
        //            if (!node.HasIndex)
        //            {
        //                if (create)
        //                {
        //                    node.Key = originalKey;
        //                    node.State = MapValueState.Filled;
        //                    return ref node;
        //                }
        //                return ref Null;
        //            }
        //            if (node.Key == originalKey)
        //            {
        //                return ref node;
        //            }
        //            if (create)
        //            {
        //                if (node.Key.CompareToRef(in originalKey) > 0)
        //                {
        //                    node.State = MapValueState.HasDefaultValue | MapValueState.Filled;
        //                    var oldKey = node.Key;
        //                    var oldValue = node.value;
        //                    node.Key = originalKey;

        //                    ref var child = ref this.GetNode(in oldKey, create);
        //                    child.Key = oldKey;
        //                    child.State = MapValueState.HasValue | MapValueState.Filled;
        //                    child.value = oldValue;
        //                    return ref node;
        //                }
        //            }
        //            current = ref node;
        //        }
        //    }
        //    if (node.Key == originalKey)
        //        return ref node;
        //    return ref Null;
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveAt(in StringSpan key)
        {
            HashedString hsKey = key;
            ref var node = ref GetNode(in hsKey);
            if(node.HasValue)
            {
                node.State = NodeState.Filled;
                node.Value = default;
                return true;
            }
            return false;
        }

        //private ref StringMap<T> GetNode(string originalKey, uint key, bool create = false, int depth = 0)
        //{
        //    ref var node = ref Null;

        //    if (Nodes == null)
        //    {
        //        if (!create)
        //        {
        //            return ref node;
        //        }
        //        Nodes = new StringMap<T>[4];
        //    }

        //    var suffix = key & 0x3;

        //    node = ref Nodes[suffix];
        //    if (!node.HasIndex)
        //    {
        //        if (create)
        //        {
        //            node.Key = originalKey;
        //            node.State = MapValueState.Filled;
        //            return ref node;
        //        }
        //        return ref Null;
        //    }

        //    if (node.Key == originalKey)
        //    {
        //        return ref node;
        //    }
        //    if (create)
        //    {
        //        // only swap bigger key
        //        // if create...
        //        if (node.Key.CompareTo(originalKey) > 0)
        //        {
        //            node.State = MapValueState.HasDefaultValue | MapValueState.Filled;
        //            var oldKey = node.Key;
        //            var oldValue = node.value;
        //            node.Key = originalKey;

        //            ref var child = ref node.GetNode(oldKey, oldKey >> (depth + 1) * 2, create, depth + 1);
        //            child.Key = oldKey;
        //            child.State = MapValueState.HasValue | MapValueState.Filled;
        //            child.value = oldValue;
        //            return ref node;
        //        }
        //    }
        //    return ref node.GetNode(originalKey, key >> 2, create, depth + 1);
        //}

    }
}
