using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core;

public struct CompactUint32Map<T>
{

    private const int Bits = 2;
    private const int Size = 1 << Bits;
    private const long Mask = ~(-1 << Bits);

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

        public bool IsNotEmpty
        {
            get
            {
                return State != 0;
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

        public IEnumerable<T> AllValues()
        {
            if (this.HasValue)
            {
                yield return this.Value;
            }
            if (this.Children == null)
            {
                yield break;
            }
            foreach (var node in this.Children)
            {
                foreach (var child in node.AllValues())
                {
                    yield return child;
                }
            }
        }
    }

    private Node[] nodes;

    public bool IsNull => nodes == null;

    public T this[uint index]
    {
        get
        {
            ref var node = ref GetNode(index);
            return node.HasValue ? node.Value : default;
        }
    }

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
        if (this.nodes == null)
        {
            yield break;
        }
        var stack = new Stack<Node>(this.nodes);
        while (stack.Count > 0)
        {
            var item = stack.Pop();
            if (item.HasValue)
            {
                yield return (item.Key, item.Value);
            }
            if (item.Children != null)
            {
                foreach (var child in item.Children)
                {
                    stack.Push(child);
                }
            }
        }
    }


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
        var storage = nodes;
        if (storage == null)
        {
            if (!create)
            {
                return ref Empty;
            }
            storage = nodes = new Node[Size];
            // special case, 0 should be filled first.
            nodes[0].State |= NodeState.Filled;
        }

        long start = originalKey;
        for (; ; )
        {
            var index = start & Mask;
            ref var node = ref storage[index];
            if (node.Key == originalKey)
            {
                if (node.IsNotEmpty)
                {
                    return ref node;
                }
            }
            if (node.Key > originalKey)
            {
                if (!create)
                {
                    return ref Empty;
                }
                var oldKey = node.Key;
                var oldValue = node.Value;
                node.Key = originalKey;
                node.Value = default;
                ref var newChild = ref GetNode(oldKey, true);
                newChild.Value = oldValue;
                newChild.Key = oldKey;
                newChild.State |= NodeState.HasValue;
                return ref node;
            }
            if (node.State == 0 && create)
            {
                node.Key = originalKey;
                node.State |= NodeState.Filled;
                return ref node;
            }

            if (node.Children == null)
            {
                if (!create)
                {
                    return ref Empty;
                }
                node.Children = new Node[Size];
            }
            storage = node.Children;
            start >>= Size;
        }
    }

    private ref Node OldGetNode(uint originalKey, bool create = false)
    {
        ref var node = ref Empty;
        var storage = nodes;
        if (storage == null)
        {
            if (!create)
            {
                return ref node;
            }
            // extend...
            storage = this.nodes = new Node[Size];
        }

        if (originalKey == 0)
        {
            node = ref storage[0];
            return ref node;
        }

        // let us walk the nodes...
        // uint offset = 0;
        // uint start = originalKey;
        long start = (originalKey << Size);
        for (; start > 0; start >>= Size)
        {
            var index = (start >> Size) & Mask;
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
                    return ref node;
                }
                node.State |= NodeState.Filled;
                if (node.Children == null)
                {
                    node.Children = new Node[Size];
                    storage = node.Children;
                    continue;
                }
            }
            if (node.Children == null)
            {
                return ref Empty;
            }
            storage = node.Children;
        }
        if (node.Key == originalKey)
        {
            return ref node;
        }
        return ref Empty;
    }

}