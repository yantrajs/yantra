using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.Core.Storage
{
    internal enum MapValueState: byte
    {
        Empty = 0,
        HasDefaultValue = 1,
        HasValue = 2,
        AnyValue = 3,
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

        public void Save(uint key, uint value)
        {
            ref var node = ref GetNode(key, key, true);
            node.State = MapValueState.HasValue;
            node.value = value;
        }

        private ref UInt32Map GetNode(uint originalKey, uint key, bool create = false)
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

            if(node.Key == originalKey)
                return ref node;

            if ((node.State | MapValueState.AnyValue) > 0)
            {
                if (node.Key > originalKey)
                {
                    // we need to shift to lower true...
                    node.State = MapValueState.HasDefaultValue;

                    ref var child = ref node.GetNode(originalKey, key >> 2, create);
                    child.Key = node.Key;
                    child.State = MapValueState.HasValue;
                    child.value = node.value;

                    node.Key = originalKey;
                }
                else
                {
                    return ref node.GetNode(originalKey, key >> 2, create);
                }
            }
            else
            {
                node.Key = originalKey;
            }
            return ref node;
        }

    }
}
