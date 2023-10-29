using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using YantraJS.Core.Core.Storage;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    public struct JSObjectProperty
    {
        public JSProperty Property;
        public uint Next;

        public static JSObjectProperty Empty;
    }

    public delegate void Updater<TKey, TValue>(TKey key, ref TValue value);

    public struct PropertySequence
    {

        public PropertyEnumerator GetEnumerator(bool showEnumerableOnly = true)
        {
            return new PropertyEnumerator(this, showEnumerableOnly);
        }

        public struct PropertyEnumerator
        {
            private SAUint32Map<JSObjectProperty> map;
            private readonly uint tail;
            private readonly bool showEnumerableOnly;
            private uint start;

            public PropertyEnumerator(PropertySequence sequence, bool showEnumerableOnly)
            {
                this.map = sequence.map;
                this.start = sequence.head;
                this.tail = sequence.tail;
                this.showEnumerableOnly = showEnumerableOnly;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext(out JSProperty property)
            {
                while (start>0)
                {
                    ref var objP = ref map.GetRefOrDefault(start, ref JSObjectProperty.Empty);
                    ref var p = ref objP.Property;
                    if (p.IsEmpty)
                    {
                        start = objP.Next;
                        continue;
                    }
                    if (showEnumerableOnly)
                    {
                        if (!p.IsEnumerable)
                        {
                            start = objP.Next;
                            continue;
                        }
                    }
                    property = p;
                    start = objP.Next;
                    return true;
                }
                property = JSProperty.Empty;
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext(out KeyString key, out JSProperty property)
            {
                while (start > 0)
                {
                    ref var objP = ref map.GetRefOrDefault(start, ref JSObjectProperty.Empty);
                    ref var p = ref objP.Property;
                    if (p.IsEmpty)
                    {
                        start = objP.Next;
                        continue;
                    }
                    if (showEnumerableOnly)
                    {
                        if (!p.IsEnumerable)
                        {
                            start = objP.Next;
                            continue;
                        }
                    }
                    property = p;
                    key = KeyStrings.GetName(start);
                    start = objP.Next;
                    return true;
                }
                property = JSProperty.Empty;
                key = KeyString.Empty;
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNextKey(out KeyString key)
            {
                while (start > 0)
                {
                    ref var objP = ref map.GetRefOrDefault(start, ref JSObjectProperty.Empty);
                    ref var p = ref objP.Property;
                    if (p.IsEmpty)
                    {
                        start = objP.Next;
                        continue;
                    }
                    if (showEnumerableOnly)
                    {
                        if (!p.IsEnumerable)
                        {
                            start = objP.Next;
                            continue;
                        }
                    }
                    key = KeyStrings.GetName(start);
                    start = objP.Next;
                    return true;
                }
                key = KeyString.Empty;
                return false;
            }
        }

        #region ValueEnumerator
        public struct ValueEnumerator
        {
            public JSObject target;
            private SAUint32Map<JSObjectProperty> map;
            private uint start;
            readonly bool showEnumerableOnly;
            public ValueEnumerator(JSObject target, bool showEnumerableOnly)
            {
                this.showEnumerableOnly = showEnumerableOnly;
                this.target = target;
                ref var properties = ref target.GetOwnProperties();
                this.map = properties.map;
                this.start = properties.head;
            }

            public bool MoveNext(out KeyString key)
            {
                while (start > 0)
                {
                    ref var objP = ref map.GetRefOrDefault(start, ref JSObjectProperty.Empty);
                    ref var p = ref objP.Property;
                    if (p.IsEmpty)
                    {
                        start = objP.Next;
                        continue;
                    }
                    if (showEnumerableOnly && !p.IsEnumerable)
                    {
                        start = objP.Next;
                        continue;
                    }
                    // property = p;
                    key = KeyStrings.GetName(start);
                    start = objP.Next;
                    return true;
                }
                // property = JSProperty.Empty;
                key = KeyString.Empty;
                return false;
            }


            public bool MoveNext(out JSValue value, out KeyString key)
            {
                while (start > 0)
                {
                    ref var objP = ref map.GetRefOrDefault(start, ref JSObjectProperty.Empty);
                    ref var p = ref objP.Property;
                    if (p.IsEmpty)
                    {
                        start = objP.Next;
                        continue;
                    }
                    if (showEnumerableOnly && !p.IsEnumerable)
                    {
                        start = objP.Next;
                        continue;
                    }
                    value = target.GetValue(in p);
                    key = KeyStrings.GetName(start);
                    start = objP.Next;
                    return true;
                }
                value = null;
                key = KeyString.Empty;
                return false;
            }

            public bool MoveNextProperty(out JSProperty value, out KeyString key)
            {
                while (start > 0)
                {
                    ref var objP = ref map.GetRefOrDefault(start, ref JSObjectProperty.Empty);
                    ref var p = ref objP.Property;
                    if (p.IsEmpty)
                    {
                        start = objP.Next;
                        continue;
                    }
                    if (showEnumerableOnly && !p.IsEnumerable)
                    {
                        start = objP.Next;
                        continue;
                    }
                    value = p;
                    key = KeyStrings.GetName(start);
                    start = objP.Next;
                    return true;
                }
                value = JSProperty.Empty;
                key = KeyString.Empty;
                return false;
            }

        }
        #endregion


        private SAUint32Map<JSObjectProperty> map;
        private uint head;
        private uint tail;

        public bool IsEmpty => head == 0;

        public void Update(Updater<uint, JSProperty> func)
        {
            var start = this.head;
            while(start > 0)
            {
                ref var objP = ref map.GetRefOrDefault(start, ref JSObjectProperty.Empty);
                ref var p = ref objP.Property;
                if (p.IsEmpty)
                {
                    start = objP.Next;
                    continue;
                }
                func(start, ref p);
                start = objP.Next;
            }
            //if (properties != null)
            //{
            //    int i = 0;
            //    foreach (var p in properties)
            //    {
            //        var update = func((p.key), p);
            //        if (update.update)
            //        {
            //            properties[i] = update.v;
            //        }
            //        i++;
            //    }
            //}
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKey(uint key)
        {
            //if (properties == null)
            //    return false;
            //if (map.IsNull)
            //{
            //    for (int i = 0; i < length; i++)
            //    {
            //        ref var p = ref properties[i];
            //        if (p.key == key && p.Attributes != JSPropertyAttributes.Empty)
            //            return true;
            //    }
            //    return false;
            //}
            return map.HasKey(key);
        }

        public bool RemoveAt(uint key)
        {
            ref var objectProperty = ref map.GetRefOrDefault(key, ref JSObjectProperty.Empty);
            ref var property = ref objectProperty.Property;
            if (property.IsEmpty)
                return false;
            if (property.IsReadOnly || !property.IsConfigurable)
                throw JSContext.Current.NewTypeError($"Cannot delete property {KeyStrings.GetNameString(key)} of {this}");
            property = JSProperty.Empty;
            return true;
            //if (properties == null)
            //    return false;
            //if (map.IsNull)
            //{
            //    for (int i = 0; i < length; i++)
            //    {
            //        ref var p = ref properties[i];
            //        if (p.key == key)
            //        {
            //            if (p.IsReadOnly || !p.IsConfigurable)
            //            {
            //                throw JSContext.Current.NewTypeError($"Cannot delete property {key} of {this}");
            //            }
            //            p = JSProperty.Empty;
            //            return true;
            //        }
            //    }
            //    return false;
            //}
            //if (map.TryRemove(key, out var pkey))
            //{
            //    ref var p = ref properties[pkey];
            //    if (p.IsReadOnly || !p.IsConfigurable)
            //    {
            //        throw JSContext.Current.NewTypeError($"Cannot delete property {key} of {this}");
            //    }
            //    // move all properties up...
            //    properties[pkey] = JSProperty.Empty;
            //}
            // return false;
        }

        public ref JSProperty GetValue(uint key)
        {
            ref var objectProperty = ref map.GetRefOrDefault(key, ref JSObjectProperty.Empty);
            ref var property = ref objectProperty.Property;
            if (property.IsEmpty)
                return ref JSProperty.Empty;
            return ref property;
            //if (properties == null)
            //    return ref JSProperty.Empty;
            //if (map.IsNull)
            //{
            //    // look up array...
            //    for (int i = 0; i < length; i++)
            //    {
            //        ref var p = ref properties[i];
            //        if(p.key == key && p.Attributes != JSPropertyAttributes.Empty)
            //        {
            //            return ref p;
            //        }
            //    }
            //} else if (map.TryGetValue(key, out var pkey))
            //{
            //    ref var obj = ref properties[pkey];
            //    if (obj.Attributes != JSPropertyAttributes.Empty)
            //        return ref obj;
            //}
            // return ref JSProperty.Empty;
        }

        public bool TryGetValue(uint key, out JSProperty obj)
        {
            ref var objectProperty = ref map.GetRefOrDefault(key, ref JSObjectProperty.Empty);
            ref var property = ref objectProperty.Property;
            if (property.IsEmpty)
            {
                obj = JSProperty.Empty;
                return false;
            }
            obj = property;
            return true;
            //if (properties == null)
            //{
            //    obj = JSProperty.Empty;
            //    return false;
            //}
            //if (map.IsNull)
            //{
            //    for (int i = 0; i < length; i++)
            //    {
            //        ref var p = ref properties[i];
            //        if (p.key == key && p.Attributes != JSPropertyAttributes.Empty)
            //        {
            //            obj = p;
            //            return true;
            //        }
            //    }
            //} else 
            //if (map.TryGetValue(key, out var pkey))
            //{
            //    obj = properties[pkey];
            //    return obj.Attributes != JSPropertyAttributes.Empty;
            //}
            //obj = new JSProperty();
            //return false;
        }
        public void Put(uint key, JSValue value, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            Put(key) = JSProperty.Property(key, value, attributes);
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
            if(head == 0)
            {
                tail = head = key;
                ref var objP = ref map.Put(key);
                return ref objP.Property;
            }

            ref var @new = ref map.Put(key);
            // when tail is same as key, it means last key was added twice..
            // it should not create a loop
            if(@new.Next == 0 && tail != key)
            {
                ref var last = ref map.GetRefOrDefault(tail, ref JSObjectProperty.Empty);
                last.Next = key;
                tail = key;
            }
            return ref @new.Property;

            //if (length < 8)
            //{
            //    for (int i = 0; i < length; i++)
            //    {
            //        ref var p = ref properties[i];
            //        if (p.key == key)
            //        {
            //            // set value..
            //            return ref properties[i];
            //        }
            //    }
            //    if (length >= properties.Length)
            //        Array.Resize(ref properties, properties.Length + 4);
            //    return ref properties[length++];
            //}
            //uint pkey;
            //if (map.IsNull)
            //{
            //    map = new UInt32Map<uint>();
            //    // copy..
            //    for (uint i = 0; i < length; i++)
            //    {
            //        ref var p = ref properties[i];
            //        map.Save(p.key, i);
            //    }
            //}
            //if (map.TryGetValue(key, out pkey))
            //{
            //    return ref properties[pkey];
            //}
            //pkey = (uint)length++;
            //if (pkey >= properties.Length)
            //{
            //    Array.Resize(ref properties, properties.Length + 4);
            //}
            //map.Save(key, pkey);
            //return ref properties[pkey];
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
                ref var p = ref this.Put(key);
                p = value;
                //if (length < 8)
                //{
                //    for (int i = 0; i < length; i++)
                //    {
                //        ref var p = ref properties[i];
                //        if (p.key == key)
                //        {
                //            // set value..
                //            properties[i] = value;
                //            return;
                //        }
                //    }
                //    if (length >= properties.Length)
                //        Array.Resize(ref properties, properties.Length + 4);
                //    properties[length++] = value;
                //    return;
                //}
                //uint pkey;
                //if (map.IsNull)
                //{
                //    map = new UInt32Map<uint>();
                //    // copy..
                //    for (uint i = 0; i < length; i++)
                //    {
                //        ref var p = ref properties[i];
                //        map.Save(p.key, i);
                //    }
                //} 
                //if (map.TryGetValue(key, out pkey))
                //{
                //    properties[pkey] = value;
                //    return;
                //}
                //pkey = (uint)length++;
                //if (pkey >= properties.Length)
                //{
                //    Array.Resize(ref properties, properties.Length + 4);
                //}
                //map.Save(key, pkey);
                //properties[pkey] = value;
            }
        }

    }
}
