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
    public struct UInt32Map
    {

        public static UInt32Map Null = new UInt32Map() { State = MapValueState.Null };

        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return State == MapValueState.Null;
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
                return (State & MapValueState.Filled) > 0;
            }
        }


        public bool HasValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (State & MapValueState.HasValue) > 0;
            }
        }

        public bool HasDefaultValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (State & MapValueState.HasDefaultValue) > 0;
            }
        }

        public IEnumerable<(uint key, uint value)> AllValues()
        {
            foreach(var node in Nodes)
            {
                if (node.Nodes != null)
                {
                    foreach (var child in node.AllValues())
                        yield return child;
                }
                yield return (node.Key, node.value);
            }
        }

        private MapValueState State;

        private UInt32Map[] Nodes;
        
        private uint value;
        private uint Key;

        public bool TryGetValue(uint key, out uint value)
        {
            ref var node = ref GetNode(key, key);
            if (!node.IsNull && node.HasValue)
            {
                value = node.value;
                return true;
            }
            value = 0;
            return false;
        }

        public bool HasKey(uint key)
        {
            ref var node = ref GetNode(key, Key);
            return node.HasValue;
        }

        public bool TryRemove(uint key, out uint value)
        {
            ref var node = ref GetNode(key, Key);
            if (node.IsNull)
            {
                value = 0;
                return false;
            }
            value = node.value;
            node.State &= MapValueState.Filled;
            return true;
        }

        public void Save(uint key, uint value)
        {
            ref var node = ref GetNode(key, key, true);
            node.State = MapValueState.HasValue | MapValueState.Filled;
            node.value = value;
        }

        private ref UInt32Map GetNode(uint originalKey, uint key, bool create = false, int depth = 0)
        {
            ref var node = ref Null;

            if (Nodes == null)
            {
                if (!create)
                {
                    return ref node;
                }
                Nodes = new UInt32Map[4];
            }

            var suffix = key & 0x3;

            node = ref Nodes[suffix];
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

                    ref var child = ref node.GetNode(oldKey, oldKey >> (depth+1)*2, create, depth + 1);
                    child.Key = oldKey;
                    child.State = MapValueState.HasValue | MapValueState.Filled;
                    child.value = oldValue;
                    return ref node;
                }
            }
            return ref node.GetNode(originalKey, key >> 2, create, depth + 1);
        }

    }
}
