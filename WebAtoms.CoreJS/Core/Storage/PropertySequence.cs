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
            List<JSProperty>.Enumerator enumerator;
            public Enumerator(PropertySequence ps)
            {
                this.enumerator = ps.properties.GetEnumerator();
            }

            public bool MoveNext()
            {
                while (this.enumerator.MoveNext())
                {
                    var current = this.enumerator.Current;
                    if (current.Attributes == JSPropertyAttributes.Deleted)
                        continue;
                    return true;
                }
                return false;
            }

            public JSProperty Current => this.enumerator.Current;
        }

        private UInt32Trie<int> map = new CompactUInt32Trie<int>();
        private List<JSProperty> properties = new List<JSProperty>();

        public IEnumerable<(uint Key, JSProperty Value)> AllValues()
        {
            foreach (var p in properties)
            {
                if (p.Attributes != JSPropertyAttributes.Deleted)
                    yield return (p.key.Key, p);
            }
        }
        public void Update(Func<uint, JSProperty, (bool update, JSProperty v)> func)
        {
            int i = 0;
            foreach (var p in properties.ToList())
            {
                var update = func((p.key.Key), p);
                if (update.update)
                {
                    properties[i] = update.v;
                }
                i++;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKey (uint key)
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
                pkey = properties.Count;
                map[key] = pkey;
                properties.Add(value);
            }
        }

    }
}
