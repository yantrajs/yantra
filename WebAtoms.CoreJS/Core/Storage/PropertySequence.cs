using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WebAtoms.CoreJS.Core
{
    public class PropertySequence
    {

        public struct Enumerator
        {
            int index;
            int size;
            JSProperty[] array;
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
                    var current = array[index];
                    if (current.Attributes == JSPropertyAttributes.Deleted)
                        continue;
                    return true;
                }
                return false;
            }

            public JSProperty Current => this.array[index];
        }

        private UInt32Trie<int> map = new CompactUInt32Trie<int>();
        private JSProperty[] properties = new JSProperty[4];

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
            return map.HasKey(key);
        }

        public bool RemoveAt(uint key)
        {
            if (map.TryRemove(key, out var pkey))
            {
                // move all properties up...
                properties[pkey] = new JSProperty { Attributes = JSPropertyAttributes.Deleted };
            }
            return false;
        }
        public bool TryGetValue(uint key, out JSProperty obj)
        {
            if (map.TryGetValue(key, out var pkey))
            {
                obj = properties[pkey];
                return obj.Attributes != JSPropertyAttributes.Deleted;
            }
            obj = new JSProperty();
            return false;
        }

        private int length = 0;

        public JSProperty this[uint key]
        {
            get
            {
                if (map.TryGetValue(key, out var pkey))
                {
                    return properties[pkey];
                }
                return new JSProperty();
            }
            set
            {
                if (map.TryGetValue(key, out var pkey))
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
            }
        }

    }
}
