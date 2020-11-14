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

        public static StringMap<T> Null = new StringMap<T>() { State = MapValueState.Null };

        private int count;

        public int Count => count;

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

        public IEnumerable<(string Key, T Value)> AllValues()
        {
            foreach (var node in Nodes)
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

        private StringMap<T>[] Nodes;

        internal T value;
        private string Key;

        public T this[string index]
        {
            get
            {
                ref var node = ref GetNode(index);
                if (node.Key == index)
                    return node.value;
                return default;
            }
            set
            {
                Save(index, value);
            }
        }

        public bool TryGetValue(string key, out T value)
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

        public bool HasKey(string key)
        {
            ref var node = ref GetNode(key);
            return node.HasValue && node.Key == key;
        }

        public bool TryRemove(string key, out T value)
        {
            ref var node = ref GetNode(key);
            if (node.HasValue)
            {
                value = node.value;
                node.State &= MapValueState.Filled;
                return true;
            }
            value = default;
            return false;
        }

        public void Save(string key, T value)
        {
            ref var node = ref GetNode(key, true);
            if (!node.HasValue) {
                count++;
            }
            node.State = MapValueState.HasValue | MapValueState.Filled;
            node.value = value;
        }

        private ref StringMap<T> GetNode(string originalKey, bool create = false)
        {
            ref var node = ref Null;
            ref var current = ref this;

            foreach(var ch in originalKey)
            {
                Int32 uch = ch;
                for (; uch > 0; uch >>= 2)
                {
                    if(current.Nodes == null)
                    {
                        if (!create)
                            return ref Null;
                        current.Nodes = new StringMap<T>[4];
                    }

                    node = ref current.Nodes[uch & 0x3];
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
                        if (node.Key.CompareTo(originalKey) > 0)
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
            }
            if (node.Key == originalKey)
                return ref node;
            return ref Null;
        }
        public bool RemoveAt(string key)
        {
            ref var node = ref GetNode(key);
            if(node.HasValue)
            {
                node.State = MapValueState.Filled;
                node.value = default;
                count--;
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
