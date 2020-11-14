using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.Core.Storage
{

    #region Old

    //public struct ElementArray
    //{

    //    private JSProperty[] Storage;
    //    private UInt32Map<uint> Map;
    //    private bool IsSparse;
    //    private uint length;
    //    public ElementArray(int size)
    //    {
    //        Storage = new JSProperty[size < 16 ? 16 : size];
    //        Map = UInt32Map<uint>.Null;
    //        IsSparse = false;
    //        length = 0;
    //    }

    //    public bool IsNull => Storage == null;

    //    public JSProperty this[uint index]
    //    {
    //        get
    //        {
    //            if(!IsSparse)
    //            {
    //                if (index < Storage.Length)
    //                    return Storage[index];
    //            }
    //            if (Map.TryGetValue(index, out var i))
    //                return Storage[i];
    //            return JSProperty.Empty;
    //        }
    //        set
    //        {
    //            uint key;
    //            if (IsSparse)
    //            {
    //                if(Map.TryGetValue(index, out key))
    //                {
    //                    Storage[key] = value;
    //                    return;
    //                }
    //                key = length++;
    //                Map.Save(index, length);
    //                if (key >= Storage.Length)
    //                {
    //                    Array.Resize(ref Storage, Storage.Length + 8);
    //                }
    //                Storage[key] = value;
    //                return;
    //            }
    //            if (index < 64)
    //            {
    //                if (index >= length)
    //                {
    //                    length = index + 1;
    //                }

    //                // extend array...
    //                if (index < Storage.Length)
    //                {
    //                    Storage[index] = value;
    //                    return;
    //                }
    //                if(index < 32)
    //                {
    //                    Array.Resize(ref Storage, 32);
    //                } else
    //                {
    //                    Array.Resize(ref Storage, 64);
    //                }
    //                Storage[index] = value;
    //                return;
    //            }

    //            // create map...
    //            IsSparse = true;
    //            key = length++;
    //            Map.Save(index, key);
    //            if (key >= Storage.Length)
    //            {
    //                Array.Resize(ref Storage, Storage.Length + 8);
    //            }
    //            Storage[key] = value;
    //        }
    //    }

    //    public bool TryGetValue(uint key, out JSProperty value)
    //    {
    //        if (IsSparse)
    //        {
    //            if (Map.TryGetValue(key, out var index))
    //            {
    //                value = Storage[index];
    //                return !value.IsEmpty;
    //            }
    //            value = JSProperty.Empty;
    //            return false;
    //        }
    //        if (key < Storage.Length)
    //        {
    //            value = Storage[key];
    //        } else
    //        {
    //            value = JSProperty.Empty;
    //            return false;
    //        }
    //        return !value.IsEmpty;
    //    }

    //    public bool TryRemove(uint key, out JSProperty value)
    //    {
    //        if (IsSparse)
    //        {
    //            if (Map.TryRemove(key, out var index))
    //            {
    //                value = Storage[index];
    //                Storage[index] = JSProperty.Empty;
    //                return true;
    //            }
    //            value = JSProperty.Empty;
    //            return false;
    //        }
    //        if (key < Storage.Length)
    //        {
    //            value = Storage[key];
    //            Storage[key] = JSProperty.Empty;
    //            return true;
    //        }
    //        value = JSProperty.Empty;
    //        return false;
    //    }
    //    public bool RemoveAt(uint key)
    //    {
    //        if (IsSparse)
    //        {
    //            if(Map.TryRemove(key,out var index))
    //            {
    //                Storage[index] = JSProperty.Empty;
    //                return true;
    //            }
    //            return false;
    //        }
    //        if (key < Storage.Length)
    //        {
    //            Storage[key] = JSProperty.Empty;
    //            return true;
    //        }
    //        return false;
    //    }
    //    public IEnumerable<(uint Key, JSProperty Value)> AllValues()
    //    {
    //        if(IsSparse)
    //        {
    //            for(uint i = 0; i < length; i++)
    //            {
    //                if(Map.TryGetValue(i, out var v))
    //                {
    //                    yield return (i , Storage[v]);
    //                }
    //            }
    //            yield break;
    //        }
    //        if (Storage == null)
    //            yield break;
    //        for (uint i = 0; i < Storage.Length; i++)
    //        {
    //            yield return (i, Storage[i]);
    //        }
    //    }
    //    public bool HasKey(uint key)
    //    {
    //        if (IsSparse)
    //        {
    //            return Map.HasKey(key);
    //        }
    //        if (key >= length)
    //        {
    //            return false;
    //        }
    //        ref var p = ref Storage[key];
    //        return !p.IsEmpty;
    //    }
    //}

    //public struct ElementArray
    //{
    //    public UInt32Map<JSProperty> storage;

    //    public JSProperty this [uint index]
    //}
    #endregion

    public struct ElementArray
    {

        private UInt32Map<JSProperty> Storage;
        private uint length;
        

        public JSProperty this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (Storage.TryGetValue(index, out var i))
                    return i;
                return JSProperty.Empty;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (index >= length)
                {
                    length = index + 1;
                }
                Storage[index] = value;
            }
        }

        public bool IsNull => !Storage.HasChildren;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(uint key, out JSProperty value)
        {
            return Storage.TryGetValue(key, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemove(uint key, out JSProperty value)
        {
            return Storage.TryRemove(key, out value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveAt(uint key)
        {
            return Storage.RemoveAt(key);
        }
        public IEnumerable<(uint Key, JSProperty Value)> AllValues()
        {
            for (uint i = 0; i < length; i++)
            {
                if (Storage.TryGetValue(i, out var v))
                {
                    yield return (i, v);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKey(uint key)
        {
            return Storage.HasKey(key);
        }
    }

}
