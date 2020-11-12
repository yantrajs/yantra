using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace YantraJS.Core.Core.Storage
{
    public struct ElementArray
    {

        private JSProperty[] Storage;
        private UInt32Map Map;
        private bool IsSparse;
        private uint length;
        public ElementArray(int size)
        {
            Storage = new JSProperty[size < 16 ? 16 : size];
            Map = UInt32Map.Null;
            IsSparse = false;
            length = 0;
        }

        public bool IsNull => Storage == null;

        public JSProperty this[uint index]
        {
            get
            {
                if(!IsSparse)
                {
                    if (index < Storage.Length)
                        return Storage[index];
                }
                if (Map.TryGetValue(index, out var i))
                    return Storage[i];
                return JSProperty.Empty;
            }
            set
            {
                uint key;
                if (IsSparse)
                {
                    if(Map.TryGetValue(index, out key))
                    {
                        Storage[key] = value;
                        return;
                    }
                    key = length++;
                    Map.Save(index, length);
                    Storage[key] = value;
                    return;
                }
                if (index < 64)
                {
                    // extend array...
                    if (index < Storage.Length)
                    {
                        Storage[index] = value;
                        return;
                    }
                    if(index < 32)
                    {
                        Array.Resize(ref Storage, 32);
                    } else
                    {
                        Array.Resize(ref Storage, 64);
                    }
                    Storage[index] = value;
                    if (index >= length)
                    {
                        length = index + 1;
                    }
                    return;
                }

                // create map...
                IsSparse = true;
                key = length++;
                Map.Save(index, key);
                Storage[key] = value;
            }
        }

        public bool TryGetValue(uint key, out JSProperty value)
        {
            if (IsSparse)
            {
                if (Map.TryGetValue(key, out var index))
                {
                    value = Storage[index];
                    return true;
                }
                value = JSProperty.Empty;
                return false;
            }
            if (key < Storage.Length)
            {
                value = Storage[key];
            } else
            {
                value = JSProperty.Empty;
            }
            return true;
        }

        public bool TryRemove(uint key, out JSProperty value)
        {
            if (IsSparse)
            {
                if (Map.TryRemove(key, out var index))
                {
                    value = Storage[index];
                    Storage[index] = JSProperty.Empty;
                    return true;
                }
                value = JSProperty.Empty;
                return false;
            }
            if (key < Storage.Length)
            {
                value = Storage[key];
                Storage[key] = JSProperty.Empty;
                return true;
            }
            value = JSProperty.Empty;
            return false;
        }
        public bool RemoveAt(uint key)
        {
            if (IsSparse)
            {
                if(Map.TryRemove(key,out var index))
                {
                    Storage[index] = JSProperty.Empty;
                    return true;
                }
                return false;
            }
            if (key < Storage.Length)
            {
                Storage[key] = JSProperty.Empty;
                return true;
            }
            return false;
        }
        public IEnumerator<(uint Key, JSProperty Value)> GetEnumerator()
        {
            if(IsSparse)
            {
                foreach(var (key, value) in Map.AllValues())
                {
                    yield return (key, Storage[value]);
                }
                yield break;
            }
            for (uint i = 0; i < Storage.Length; i++)
            {
                yield return (i, Storage[i]);
            }
        }
    }
}
