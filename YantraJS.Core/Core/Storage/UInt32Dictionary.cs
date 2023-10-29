using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace YantraJS.Core.Core.Storage
{
    internal enum MapValueState : byte
    {
        Empty = 0,
        Filled = 1,
        HasDefaultValue = 2,
        HasValue = 4,
        AnyValue = 6,
        Null = 0xFF
    }

    ///// <summary>
    ///// Mapping of uint to uint
    ///// </summary>
    //public struct UInt32Map<T>
    //{

    //    [DebuggerDisplay("{Key}: {Value}")]
    //    public struct KeyValue
    //    {
    //        public uint Key;
    //        public T Value;
    //    }


    //    public static UInt32Map<T> Null = new UInt32Map<T>() { State = MapValueState.Null };

    //    public bool IsNull
    //    {
    //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //        get
    //        {
    //            return Nodes == null || State == MapValueState.Null;
    //        }
    //    }

    //    public bool HasChildren
    //    {
    //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //        get
    //        {
    //            return Nodes != null;
    //        }
    //    }


    //    public bool IsEmpty
    //    {
    //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //        get
    //        {
    //            return State == MapValueState.Empty;
    //        }
    //    }

    //    public bool HasIndex
    //    {
    //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //        get
    //        {
    //            return (State & MapValueState.Filled) > 0 && State != MapValueState.Null;
    //        }
    //    }


    //    public bool HasValue
    //    {
    //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //        get
    //        {
    //            return (State & MapValueState.HasValue) > 0 && State != MapValueState.Null;
    //        }
    //    }

    //    public bool HasDefaultValue
    //    {
    //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //        get
    //        {
    //            return (State & MapValueState.HasDefaultValue) > 0 && State != MapValueState.Null;
    //        }
    //    }

    //    public IEnumerable<KeyValue> All
    //    {
    //        get
    //        {
    //            foreach (var (k,v) in AllValues())
    //                yield return new KeyValue { Key = k, Value = v };
    //        }
    //    }

    //    public IEnumerable<(uint Key, T Value)> AllValues()
    //    {
    //        if (Nodes != null)
    //        {
    //            (uint Key, T Value) pair;
    //            for (int i = 0; i < 4; i++)
    //            {
    //                if (TryGetAt(i, out pair))
    //                {
    //                    yield return pair;
    //                }
    //                foreach (var item in AllValues(i))
    //                {
    //                    yield return item;
    //                }
    //            }
    //        }
    //    }

    //    private IEnumerable<(uint Key, T Value)> AllValues(int i)
    //    {
    //        ref var node = ref Nodes[i];
    //        return node.AllValues();
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    private bool TryGetAt(int index, out (uint Key, T Value) pair)
    //    {
    //        ref var node = ref Nodes[index];
    //        if (node.HasValue)
    //        {
    //            pair = (node.Key, node.value);
    //            return true;
    //        }
    //        pair = (0, default);
    //        return false;
    //    }

    //    private MapValueState State;

    //    private UInt32Map<T>[] Nodes;

    //    private uint Key;
    //    private T value;

    //    public T this[uint key]
    //    {
    //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //        get
    //        {
    //            ref var node = ref GetNode(key);
    //            if (node.HasValue)
    //                return node.value;
    //            return default;
    //        }
    //        [Obsolete("Use Put method")]
    //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //        set
    //        {
    //            Save(key, value);
    //        }
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool TryGetValue(uint key, out T value)
    //    {
    //        ref var node = ref GetNode(key);
    //        if (node.HasValue)
    //        {
    //            value = node.value;
    //            return true;
    //        }
    //        value = default;
    //        return false;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool HasKey(uint key)
    //    {
    //        ref var node = ref GetNode(key);
    //        return node.HasValue;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool TryRemove(uint key, out T value)
    //    {
    //        ref var node = ref GetNode(key);
    //        if (!node.HasValue)
    //        {
    //            value = default;
    //            return false;
    //        }
    //        value = node.value;
    //        node.State &= MapValueState.Filled;
    //        return true;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void Save(uint key, T value)
    //    {
    //        ref var node = ref GetNode(key, true);
    //        node.State = MapValueState.HasValue | MapValueState.Filled;
    //        node.value = value;
    //    }

    //    public ref T Put(uint key) {
    //        ref var node = ref GetNode(key, true);
    //        node.State = MapValueState.HasValue | MapValueState.Filled;
    //        return ref node.value;
    //    }

    //    public ref T GetRefOrDefault(uint key, ref T def)
    //    {
    //        ref var node = ref GetNode(key, false);
    //        if(node.HasValue)
    //        {
    //            return ref node.value;
    //        }
    //        return ref def;
    //    }

    //    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    private ref UInt32Map<T> GetNode(uint originalKey, bool create = false)
    //    {
    //        ref var node = ref Null;
    //        ref var current = ref this;

    //        if (originalKey == 0)
    //        {
    //            if (Nodes == null && !create)
    //            {
    //                return ref Null;
    //            }
    //            Nodes = Nodes ?? new UInt32Map<T>[4];
    //            node = ref Nodes[0];
    //            if (node.Key != 0)
    //            {
    //                // move...
    //                var oldKey = node.Key;
    //                var oldValue = node.value;
    //                var oldState = node.State;
    //                node.Key = 0;
    //                node.State = MapValueState.HasDefaultValue | MapValueState.Filled;
    //                node.value = default;

    //                ref var child = ref GetNode(oldKey, true);
    //                child.Key = oldKey;
    //                child.value = oldValue;
    //                child.State = oldState;
    //            }
    //            node.State |= MapValueState.Filled;
    //            return ref node;
    //        }

    //        for (long key = originalKey; key > 0; key >>= 2)
    //        {
    //            if (current.Nodes == null)
    //            {
    //                if (!create)
    //                {
    //                    return ref Null;
    //                }
    //                current.Nodes = new UInt32Map<T>[4];
    //            }

    //            node = ref current.Nodes[key & 0x3];
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
    //                // only swap bigger key
    //                // if create...
    //                if (node.Key > originalKey)
    //                {
    //                    var oldKey = node.Key;
    //                    var oldValue = node.value;
    //                    node.Key = originalKey;
    //                    node.value = default;
    //                    node.State = MapValueState.HasDefaultValue | MapValueState.Filled;

    //                    ref var child = ref this.GetNode(oldKey, create);
    //                    child.Key = oldKey;
    //                    child.State = MapValueState.HasValue | MapValueState.Filled;
    //                    child.value = oldValue;
    //                    return ref node;
    //                }
    //            }
    //            current = ref node;
    //        }
    //        if (node.Key == originalKey)
    //            return ref node;
    //        return ref Null;
    //    }
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool RemoveAt(uint key)
    //    {
    //        ref var node = ref GetNode(key);
    //        if (node.HasValue)
    //        {
    //            node.State = MapValueState.Filled;
    //            node.value = default;
    //            return true;
    //        }
    //        return false;
    //    }

    //}
}
