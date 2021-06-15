using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using YantraJS.Core.Core.Storage;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    public struct PropertySequence
    {

        #region ValueEnumerator
        public struct ValueEnumerator
        {
            public JSObject target;
            int index;
            readonly JSProperty[] array;
            readonly int size;
            readonly bool enumerableOnly;
            public ValueEnumerator(JSObject target, bool showEnumerableOnly)
            {
                enumerableOnly = showEnumerableOnly;
                index = -1;
                this.target = target;
                ref var op = ref target.GetOwnProperties(false);
                var array = op.properties;
                this.array = array;
                this.size = array?.Length ?? 0;
            }

            public bool MoveNext(out KeyString key)
            {
                if (this.array != null)
                {
                    while ((++index) < size)
                    {
                        ref var current = ref array[index];
                        //if (current.Attributes == JSPropertyAttributes.Empty)
                        //    continue;
                        if (current.IsEmpty)
                            continue;
                        if (enumerableOnly)
                        {
                            if (!current.IsEnumerable)
                                continue;
                        }
                        key = current.key;
                        return true;
                    }

                }
                key = KeyString.Empty;
                return false;
            }


            public bool MoveNext(out JSValue value, out KeyString key)
            {
                if (this.array != null)
                {
                    while ((++index) < size)
                    {
                        ref var current = ref array[index];
                        //if (current.Attributes == JSPropertyAttributes.Deleted)
                        //    continue;
                        if (current.IsEmpty)
                            continue;
                        if (enumerableOnly)
                        {
                            if (!current.IsEnumerable)
                                continue;
                        }
                        value = target.GetValue(current);
                        key = current.key;
                        return true;
                    }

                }
                value = null;
                key = KeyString.Empty;
                return false;
            }

            public bool MoveNextProperty(out JSProperty value, out KeyString key)
            {
                if (this.array != null)
                {
                    while ((++index) < size)
                    {
                        ref var current = ref array[index];
                        //if (current.Attributes == JSPropertyAttributes.Deleted)
                        //    continue;
                        if (current.IsEmpty)
                            continue;
                        if (enumerableOnly)
                        {
                            if (!current.IsEnumerable)
                                continue;
                        }
                        value = current;
                        key = current.key;
                        return true;
                    }

                }
                value = new JSProperty { };
                key = KeyString.Empty;
                return false;
            }

        }
        #endregion

        #region Enumerator
        public struct Enumerator
        {
            int index;
            readonly int size;
            readonly JSProperty[] array;
            public Enumerator(PropertySequence ps)
            {
                index = -1;
                array = ps.properties;
                size = ps.length;
            }

            public bool MoveNext()
            {
                if (array == null)
                    return false;
                while ((++index) < size)
                {
                    ref var current = ref array[index];
                    if (current.Attributes == JSPropertyAttributes.Empty)
                        continue;
                    return true;
                }
                return false;
            }

            public ref JSProperty Current => ref this.array[index];
        }
        #endregion



        private UInt32Map<uint> map;
        internal JSProperty[] properties;
        private int length;

        public PropertySequence(int size)
        {
            this.length = 0;
            map = UInt32Map<uint>.Null;
            properties = new JSProperty[size];
        }

        public bool IsEmpty => properties == null;

        public IEnumerable<(uint Key, JSProperty Value)> AllValues()
        {
            if (properties != null)
            {
                foreach (var p in properties)
                {
                    if (p.Attributes != JSPropertyAttributes.Empty)
                        yield return (p.key.Key, p);
                }
            }
        }
        public void Update(Func<uint, JSProperty, (bool update, JSProperty v)> func)
        {
            if (properties != null)
            {
                int i = 0;
                foreach (var p in properties)
                {
                    var update = func((p.key.Key), p);
                    if (update.update)
                    {
                        properties[i] = update.v;
                    }
                    i++;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKey(uint key)
        {
            if (properties == null)
                return false;
            if (map.IsNull)
            {
                for (int i = 0; i < length; i++)
                {
                    ref var p = ref properties[i];
                    if (p.key.Key == key && p.Attributes != JSPropertyAttributes.Empty)
                        return true;
                }
                return false;
            }
            return map.HasKey(key);
        }

        public bool RemoveAt(uint key)
        {
            if (properties == null)
                return false;
            if (map.IsNull)
            {
                for (int i = 0; i < length; i++)
                {
                    ref var p = ref properties[i];
                    if (p.key.Key == key)
                    {
                        if (p.IsReadOnly || !p.IsConfigurable)
                        {
                            throw JSContext.Current.NewTypeError($"Cannot delete property {key} of {this}");
                        }
                        p = JSProperty.Empty;
                        return true;
                    }
                }
                return false;
            }
            if (map.TryRemove(key, out var pkey))
            {
                ref var p = ref properties[pkey];
                if (p.IsReadOnly || !p.IsConfigurable)
                {
                    throw JSContext.Current.NewTypeError($"Cannot delete property {key} of {this}");
                }
                // move all properties up...
                properties[pkey] = JSProperty.Empty;
            }
            return false;
        }

        public ref JSProperty GetValue(uint key)
        {
            if (properties == null)
                return ref JSProperty.Empty;
            if (map.IsNull)
            {
                // look up array...
                for (int i = 0; i < length; i++)
                {
                    ref var p = ref properties[i];
                    if(p.key.Key == key && p.Attributes != JSPropertyAttributes.Empty)
                    {
                        return ref p;
                    }
                }
            } else if (map.TryGetValue(key, out var pkey))
            {
                ref var obj = ref properties[pkey];
                if (obj.Attributes != JSPropertyAttributes.Empty)
                    return ref obj;
            }
            return ref JSProperty.Empty;
        }

        public bool TryGetValue(uint key, out JSProperty obj)
        {
            if (properties == null)
            {
                obj = JSProperty.Empty;
                return false;
            }
            if (map.IsNull)
            {
                for (int i = 0; i < length; i++)
                {
                    ref var p = ref properties[i];
                    if (p.key.Key == key && p.Attributes != JSPropertyAttributes.Empty)
                    {
                        obj = p;
                        return true;
                    }
                }
            } else 
            if (map.TryGetValue(key, out var pkey))
            {
                obj = properties[pkey];
                return obj.Attributes != JSPropertyAttributes.Empty;
            }
            obj = new JSProperty();
            return false;
        }

        public void Put(in KeyString key, JSValue value, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            Put(key.Key) = JSProperty.Property(key, value, attributes);
        }

        public void Put(in KeyString key, JSFunction getter, JSFunction setter, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            Put(key.Key) = JSProperty.Property(key, getter, setter, attributes);
        }

        public void Put(in KeyString key, JSFunctionDelegate getter, JSFunctionDelegate setter, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            Put(key.Key) = JSProperty.Property(key, getter, setter, attributes);
        }

        /// <summary>
        /// Put method is faster as runtime will initiate the object in place instead of
        /// creating it in local variable and put it on stack for another method
        /// 
        /// https://sharplab.io/#v2:C4LghgzgtgPgAgJgIwFgBQcDMACR2DC2A3utmbjnACzYCyAFAJTGnlsBuYATtmNgLzYAdgFMA7tgCCTANys25MADoAyiOD1REgFIqAClwD2ABxFdgAT3pIANAkaM5aBeXnYAvm7dZcNWgiYWZxdsTh4+QS0pWTc2ZQBxdUDI8WxdAxMzS3oqGwBWRzdPYLJvSgQpIJCfSRUAHnSjU3MLAD5eJxDsWPIAbTgkJQAlAFchYABLKBElfEMoYwmAGzM1LnYJgGMRCCVadQALQwATAEkFpfoB4bHJ6dn5xZWuNY3t3f3gI7OLgHljSaGIS7SQAc1BXB2EAm7BEpyESwmQiRoMYAF0ehRsJCAGZpfRNLIWbCJDTMEglLpwADs2JEeOUtE6LmKXX6g1G4ymMzmC2WqzMbx2e0OJ3Oxku105dx5j35L0FW2Fn2+4qW/0BwKUYIhUJhcIRSJR6MxWOo2DUGiR+IyzUsoTASxGInJpsUewEDqdImZLKKXkpPggwC4I02wCk9QAKu0KVScFG6L6PAG2D5sMHQ+GbYSWlUXD4kRGAGqO53JtM4IsW4BgYAjCAVvpS27ch5856vJUfUU/CVXDmt+68p4C9bdkVfMV/AETIEg8GQiDQ2HwxHIoSojGUys5zItejV9g2bDViCuncuUvez3sJsuFS1+sQT2N02s8jFdxAA==
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ref JSProperty Put(uint key)
        {
            if (length < 8)
            {
                for (int i = 0; i < length; i++)
                {
                    ref var p = ref properties[i];
                    if (p.key.Key == key)
                    {
                        // set value..
                        return ref properties[i];
                    }
                }
                if (length >= properties.Length)
                    Array.Resize(ref properties, properties.Length + 4);
                return ref properties[length++];
            }
            uint pkey;
            if (map.IsNull)
            {
                map = new UInt32Map<uint>();
                // copy..
                for (uint i = 0; i < length; i++)
                {
                    ref var p = ref properties[i];
                    map.Save(p.key.Key, i);
                }
            }
            if (map.TryGetValue(key, out pkey))
            {
                return ref properties[pkey];
            }
            pkey = (uint)length++;
            if (pkey >= properties.Length)
            {
                Array.Resize(ref properties, properties.Length + 4);
            }
            map.Save(key, pkey);
            return ref properties[pkey];
        }

        public JSProperty this[uint key]
        {
            //get
            //{
            //    if (map != null && map.TryGetValue(key, out var pkey))
            //    {
            //        return properties[pkey];
            //    }
            //    return JSProperty.Empty;
            //}
            [Obsolete("Use Put")]
            set
            {
                if (length < 8)
                {
                    for (int i = 0; i < length; i++)
                    {
                        ref var p = ref properties[i];
                        if (p.key.Key == key)
                        {
                            // set value..
                            properties[i] = value;
                            return;
                        }
                    }
                    if (length >= properties.Length)
                        Array.Resize(ref properties, properties.Length + 4);
                    properties[length++] = value;
                    return;
                }
                uint pkey;
                if (map.IsNull)
                {
                    map = new UInt32Map<uint>();
                    // copy..
                    for (uint i = 0; i < length; i++)
                    {
                        ref var p = ref properties[i];
                        map.Save(p.key.Key, i);
                    }
                } 
                if (map.TryGetValue(key, out pkey))
                {
                    properties[pkey] = value;
                    return;
                }
                pkey = (uint)length++;
                if (pkey >= properties.Length)
                {
                    Array.Resize(ref properties, properties.Length + 4);
                }
                map.Save(key, pkey);
                properties[pkey] = value;
            }
        }

    }
}
