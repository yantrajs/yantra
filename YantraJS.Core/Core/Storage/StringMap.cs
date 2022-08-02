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


        public static StringMap<T> Null = new StringMap<T>() { State = MapValueState.Null };

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

        //struct Iterator : IEnumerator<(string Key, T Value)>
        //{

        //    StringMap<T>[] array;
        //    int index;
        //    public Iterator(StringMap<T>[] array)
        //    {
        //        this.array = array;
        //        this.index = -1;
        //    }

        //    public (string Key, T Value) Current {
        //        get
        //        {
        //            ref var node = ref array[index];
        //            return (node.Key.Value, node.value);
        //        }
        //    }

        //    object IEnumerator.Current => Current;

        //    public void Dispose()
        //    {
                
        //    }

        //    public bool MoveNext()
        //    {
        //        return (this.index++) < this.array.Length;
        //    }

        //    public void Reset()
        //    {
                
        //    }
        //}

        public IEnumerable<(StringSpan Key, T Value)> AllValues()
        {
            if (Nodes != null)
            {
                (StringSpan Key, T Value) pair;
                for (int i = 0; i < Size; i++)
                {
                    if(TryGetAt(i, out pair)) {
                        yield return pair;
                    }
                    foreach (var item in AllValues(i))
                    {
                        yield return item;
                    }
                }
            } 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<(StringSpan Key, T Value)> AllValues(int i)
        {
            ref var node = ref Nodes[i];
            return node.AllValues();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetAt(int index, out (StringSpan Key, T Value) pair)
        {
            ref var node = ref Nodes[index];
            if (node.HasValue)
            {
                pair = (node.Key.Value, node.value);
                return true;
            }
            pair = (null, default);
            return false;
        }

        private MapValueState State;

        private StringMap<T>[] Nodes;

        internal T value;
        private HashedString Key;

        public T this[in HashedString index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref var node = ref GetNode(index);
                if (node.HasValue)
                    return node.value;
                return default;
            }
            //[Obsolete("Use Put")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                ref var node = ref GetNode(index, true);
                node.State = MapValueState.HasValue | MapValueState.Filled;
                node.value = value;

            }
        }

        public ref T Put(in HashedString index)
        {
            ref var node = ref GetNode(index, true);
            node.State = MapValueState.HasValue | MapValueState.Filled;
            return ref node.value;
        }


        public T this[in StringSpan index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref var node = ref GetNode(index);
                if (node.HasValue)
                    return node.value;
                return default;
            }
            //[Obsolete("Use Put")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                ref var node = ref GetNode(index, true);
                node.State = MapValueState.HasValue | MapValueState.Filled;
                node.value = value;

            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(in StringSpan key, out T value)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(in HashedString key, out T value)
        {
            ref var node = ref GetNode(in key);
            if (node.HasValue)
            {
                value = node.value;
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
                value = node.value;
                node.State &= MapValueState.Filled;
                return true;
            }
            value = default;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Save(in HashedString key, T value)
        {
            ref var node = ref GetNode(in key, true);
            node.State = MapValueState.HasValue | MapValueState.Filled;
            node.value = value;
        }

        private ref StringMap<T> GetNode(in HashedString originalKey, bool create = false)
        {
            ref var node = ref Null;
            ref var current = ref this;

            int start = originalKey.Hash;
            for(; start != 0; start >>= Bits)
            {
                if(current.Nodes == null)
                {
                    if (!create)
                        return ref Null;
                    current.Nodes = new StringMap<T>[Size];
                }
                node = ref current.Nodes[start & Mask];
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
                if (create) {
                    if (node.Key.CompareToRef(in originalKey) > 0)
                    {
                        node.State = MapValueState.HasDefaultValue | MapValueState.Filled;
                        var oldKey = node.Key;
                        var oldValue = node.value;
                        node.Key = originalKey;
                        ref var child = ref this.GetNode(in oldKey, true);
                        child.Key = oldKey;
                        child.State = MapValueState.HasValue | MapValueState.Filled;
                        child.value = oldValue;
                        return ref node;
                    }
                }
                current = ref node;
            }
            if (node.Key == originalKey)
            {
                return ref node;
            }
            var en = originalKey.Value.GetEnumerator();
            while(en.MoveNext(out var ch))
            {
                Int32 uch = ch;
                for (; uch > 0; uch >>= Bits)
                {
                    if(current.Nodes == null)
                    {
                        if (!create)
                            return ref Null;
                        current.Nodes = new StringMap<T>[Size];
                    }

                    node = ref current.Nodes[uch & Mask];
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
                        if (node.Key.CompareToRef(in originalKey) > 0)
                        {
                            node.State = MapValueState.HasDefaultValue | MapValueState.Filled;
                            var oldKey = node.Key;
                            var oldValue = node.value;
                            node.Key = originalKey;

                            ref var child = ref this.GetNode(in oldKey, create);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveAt(in StringSpan key)
        {
            HashedString hsKey = key;
            ref var node = ref GetNode(in hsKey);
            if(node.HasValue)
            {
                node.State = MapValueState.Filled;
                node.value = default;
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
