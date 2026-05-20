using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.FastParser;
using YantraJS.Core.Storage;

namespace YantraJS.Core;


public struct Uint32Map<T>
{

    private const int Bits = 4;
    private const int Size = 1 << Bits;
    private const int Mask = ~(-1 << Bits);

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
        public Node[] Children;
    }

    private Node[] nodes;

    public bool IsNull => nodes == null;

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

    public ref T GetRefOrDefault(uint key, ref T def)
    {
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
        ref var storage = ref nodes;
        if (storage == null)
        {
            if (!create)
            {
                return ref node;
            }
            // extend...
            storage = new Node[Size];
        }

        if (originalKey == 0)
        {
            node = ref storage[0];
            return ref node;
        }

        // let us walk the nodes...
        uint offset = 0;
        uint start = originalKey;
        for (; start != 0; start >>= Size)
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
                if (node.Key > originalKey)
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
                if (node.Children == null)
                {
                    node.Children = new Node[Size];
                }
            }
            if (node.Children == null)
            {
                return ref Empty;
            }
            storage = ref node.Children;
        }
        if (node.Key == originalKey)
        {
            return ref node;
        }
        return ref Empty;
    }

}

