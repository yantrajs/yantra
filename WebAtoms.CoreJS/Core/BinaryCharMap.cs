using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("WebAtoms.CoreJS.Tests")]
namespace WebAtoms.CoreJS.Core {

    internal class CharMapValue<T>
    {
        public string Key;
        public T Value;
    }

    internal class BinaryCharMap<T>: BaseMap<string, T>
    {

        private uint next = 16;

        private uint grow = 64;

        private static UInt16 first = (UInt16)0xF000;
        private static UInt16 second = (UInt16)0xF00;
        private static UInt16 third = (UInt16)0xF0;
        private static UInt16 fourth = (UInt16)0xF;

        public BinaryCharMap()
        {
            Buffer = new TrieNode[grow];
        }

        protected override IEnumerable<KeyValuePair<string, T>> Enumerate(uint index)
        {
            var last = index + 16;
            for (uint i = index; i < last; i++)
            {
                var node = Buffer[i];
                var fi = node.FirstChildIndex;
                var v = node.Value;
                if (v != null)
                {
                    yield return new KeyValuePair<string, T>(v.Key, v.Value);
                }
                if (fi == 0)
                {
                    continue;
                }
                foreach (var a in Enumerate(fi)) yield return a;
            }
        }

        public T this[string input]
        {
            get
            {
                if (this.TryGetValue(input, out var t))
                    return t;
                return default;
            }
            set
            {
                this.Save(input, value);
            }
        }

        public bool TryGetValue(string key, out T value)
        {
            ref var node = ref GetTrieNode(key);
            if (node.Value != null)
            {
                value = node.Value.Value;
                return true;
            }
            value = default;
            return false;
        }

        public T GetOrCreate(string key, Func<T> factory)
        {
            ref var node = ref GetTrieNode(key, true);
            if (node.Value != null)
            {
                return node.Value.Value;
            }
            var v = factory();
            node.Value = new NodeValue { Key = key, Value = v };
            return v;
        }

        public bool Remove(string key)
        {
            ref var node = ref GetTrieNode(key, false);
            if (node.Value != null)
            {
                node.Value = null;
                return true;
            }
            return false;
        }

        public void Save(string key, T value)
        {
            ref var node = ref GetTrieNode(key, true);
            node.Value = new NodeValue { Key = key, Value = value };
        }

        private ref TrieNode GetTrieNode(IEnumerable<char> key, bool create = false)
        {
            uint index = 0;
            foreach (var ch in key)
            {
                UInt16 uv = (UInt16)ch;
                byte b1 = (byte)((uv & first) >> 12);
                byte b2 = (byte)((uv & second) >> 8);
                byte b3 = (byte)((uv & third) >> 4);
                byte b4 = (byte)(uv & fourth);

                index = GetNode(index + b1, create);
                index = GetNode(index + b2, create);
                index = GetNode(index + b3, create);
                index = GetNode(index + b4, create);
                if (index == uint.MaxValue)
                {
                    return ref TrieNode.Empty;
                }
            }

            return ref Buffer[index];

        }

        private uint GetNode(uint position, bool create = false)
        {
            if (position >= Buffer.Length)
            {
                if (!create)
                {
                    return uint.MaxValue;
                }
                this.EnsureCapacity(position);
            }
            ref var v = ref Buffer[position];
            if (v.FirstChildIndex == 0)
            {
                if (!create)
                {
                    return uint.MaxValue;
                }
                v.FirstChildIndex = next;
                next += 16;
                this.EnsureCapacity(v.FirstChildIndex);
            }
            return v.FirstChildIndex;
            // return ref Buffer[v.FirstChildIndex];
            // return v.FirstChildIndex;
        }

        void EnsureCapacity(uint i1)
        {
            if (this.Buffer.Length <= i1)
            {
                // add 16  more...
                var b = new TrieNode[i1 + grow];
                Array.Copy(this.Buffer, b, this.Buffer.Length);
                this.Buffer = b;
            }
        }
    }

}