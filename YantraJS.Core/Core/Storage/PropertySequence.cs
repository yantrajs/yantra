using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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
                        if (current.Attributes == JSPropertyAttributes.Deleted)
                            continue;
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
                        if (current.Attributes == JSPropertyAttributes.Deleted)
                            continue;
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
                        if (current.Attributes == JSPropertyAttributes.Deleted)
                            continue;
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
                    if (current.Attributes == JSPropertyAttributes.Deleted)
                        continue;
                    return true;
                }
                return false;
            }

            public ref JSProperty Current => ref this.array[index];
        }
        #endregion



        private UInt32Trie<int> map;
        private JSProperty[] properties;
        private int length;

        public PropertySequence(int size)
        {
            this.length = 0;
            map = null;
            properties = new JSProperty[size];
        }

        public bool IsEmpty => properties == null;

        public IEnumerable<(uint Key, JSProperty Value)> AllValues()
        {
            if (properties != null)
            {
                foreach (var p in properties)
                {
                    if (p.Attributes != JSPropertyAttributes.Deleted)
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
            if (map == null)
            {
                for (int i = 0; i < length; i++)
                {
                    ref var p = ref properties[i];
                    if (p.key.Key == key && p.Attributes != JSPropertyAttributes.Deleted)
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
            if (map == null)
            {
                for (int i = 0; i < length; i++)
                {
                    ref var p = ref properties[i];
                    if (p.key.Key == key)
                    {
                        p.Attributes = JSPropertyAttributes.Deleted;
                        return true;
                    }
                }
                return false;
            }
            if (map.TryRemove(key, out var pkey))
            {
                // move all properties up...
                properties[pkey] = new JSProperty { Attributes = JSPropertyAttributes.Deleted };
            }
            return false;
        }

        public ref JSProperty GetValue(uint key)
        {
            if (properties == null)
                return ref JSProperty.Empty;
            if (map == null)
            {
                // look up array...
                for (int i = 0; i < length; i++)
                {
                    ref var p = ref properties[i];
                    if(p.key.Key == key && p.Attributes != JSPropertyAttributes.Deleted)
                    {
                        return ref p;
                    }
                }
            } else if (map.TryGetValue(key, out var pkey))
            {
                ref var obj = ref properties[pkey];
                if (obj.Attributes != JSPropertyAttributes.Deleted)
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
            if (map == null)
            {
                for (int i = 0; i < length; i++)
                {
                    ref var p = ref properties[i];
                    if (p.key.Key == key && p.Attributes != JSPropertyAttributes.Deleted)
                    {
                        obj = p;
                        return true;
                    }
                }
            } else if (map.TryGetValue(key, out var pkey))
            {
                obj = properties[pkey];
                return obj.Attributes != JSPropertyAttributes.Deleted;
            }
            obj = new JSProperty();
            return false;
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
                int pkey;
                if (map == null)
                {
                    map = new UInt32Trie<int>(16);
                    // copy..
                    for (int i = 0; i < length; i++)
                    {
                        ref var p = ref properties[i];
                        map[p.key.Key] = i;
                    }
                } 
                if (map.TryGetValue(key, out pkey))
                {
                    properties[pkey] = value;
                    return;
                }
                pkey = length++;
                if (pkey >= properties.Length)
                {
                    Array.Resize(ref properties, properties.Length + 4);
                }
                map[key] = pkey;
                properties[pkey] = value;
                length++;
            }
        }

    }
}
