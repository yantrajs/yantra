using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace YantraJS.Core.Core.Storage
{
    internal enum MapValueState: byte
    {
        Empty = 0,
        Filled = 1,
        HasDefaultValue = 2,
        HasValue = 4,
        AnyValue = 6,
        Null = 0xFF
    }

    /// <summary>
    /// Mapping of uint to uint
    /// </summary>
    public struct UInt32Map<T>
    {

        public static UInt32Map<T> Null = new UInt32Map<T>() { State = MapValueState.Null };

        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Nodes == null || State == MapValueState.Null;
            }
        }

        public bool HasChildren
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Nodes != null;
            }
        }


        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return State == MapValueState.Empty;
            }
        }

        public bool HasIndex
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (State & MapValueState.Filled) > 0 && State != MapValueState.Null;
            }
        }


        public bool HasValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (State & MapValueState.HasValue) > 0 && State != MapValueState.Null;
            }
        }

        public bool HasDefaultValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (State & MapValueState.HasDefaultValue) > 0 && State != MapValueState.Null;
            }
        }

        public IEnumerable<(uint Key, T Value)> AllValues()
        {
            foreach(var node in Nodes)
            {
                if (node.Nodes != null)
                {
                    foreach (var child in node.AllValues())
                        yield return child;
                }
                if (node.HasValue)
                {
                    yield return (node.Key, node.value);
                }
            }
        }

        private MapValueState State;

        private UInt32Map<T>[] Nodes;

        private uint Key;
        private T value;

        public T this[uint key]
        {
            get
            {
                ref var node = ref GetNode(key);
                if (node.HasValue)
                    return node.value;
                return default;
            }
            set
            {
                Save(key, value);
            }
        }

        public bool TryGetValue(uint key, out T value)
        {
            ref var node = ref GetNode(key);
            if (node.HasValue)
            {
                value = node.value;
                return true;
            }
            value = default;
            return false;
        }

        public bool HasKey(uint key)
        {
            ref var node = ref GetNode(key);
            return node.HasValue;
        }

        public bool TryRemove(uint key, out T value)
        {
            ref var node = ref GetNode(key);
            if (node.IsNull)
            {
                value = default;
                return false;
            }
            value = node.value;
            node.State &= MapValueState.Filled;
            return true;
        }

        public void Save(uint key, T value)
        {
            ref var node = ref GetNode(key, true);
            if (node.State == MapValueState.Null) {
                throw new InsufficientMemoryException();
            }
            node.State = MapValueState.HasValue | MapValueState.Filled;
            node.value = value;
        }

        private ref UInt32Map<T> GetNode(uint originalKey, bool create = false)
        {
            ref var node = ref Null;
            ref var current = ref this;

            if (originalKey == 0)
            {
                if (Nodes == null && !create)
                {
                    return ref Null;
                }
                Nodes = Nodes ?? new UInt32Map<T>[4];
                node = ref Nodes[0];
                if (node.Key != 0)
                {
                    // move...
                    var oldKey = node.Key;
                    var oldValue = node.value;
                    var oldState = node.State;
                    node.Key = 0;
                    node.State = MapValueState.HasDefaultValue | MapValueState.Filled;
                    node.value = default;

                    ref var child = ref GetNode(oldKey, true);
                    child.Key = oldKey;
                    child.value = oldValue;
                    child.State = oldState;
                }
                node.State |= MapValueState.Filled;
                return ref node;
            }

            for (long key = originalKey; key > 0; key >>= 2)
            {
                if (current.Nodes == null)
                {
                    if (!create)
                    {
                        return ref Null;
                    }
                    current.Nodes = new UInt32Map<T>[4];
                }

                node = ref current.Nodes[key & 0x3];
                if (!node.HasIndex)
                {
                    if (create)
                    {
                        node.Key = originalKey;
                        node.State = MapValueState.Filled;
                        return ref node;
                    }
                    return ref Null;
                }

                if (node.Key == originalKey)
                {
                    return ref node;
                }
                if (create)
                {
                    // only swap bigger key
                    // if create...
                    if (node.Key > originalKey)
                    {
                        node.State = MapValueState.HasDefaultValue | MapValueState.Filled;
                        var oldKey = node.Key;
                        var oldValue = node.value;
                        node.Key = originalKey;

                        ref var child = ref this.GetNode(oldKey, create);
                        child.Key = oldKey;
                        child.State = MapValueState.HasValue | MapValueState.Filled;
                        child.value = oldValue;
                        return ref node;
                    }
                }
                current = ref node;
            }
            if (node.Key == originalKey)
                return ref node;
            return ref Null;
        }
        public bool RemoveAt(uint key)
        {
            ref var node = ref GetNode(key);
            if (node.HasValue)
            {
                node.State = MapValueState.Filled;
                node.value = default;
                return true;
            }
            return false;
        }

    }
}
