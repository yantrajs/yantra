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

        public override int Update(Func<string, T, (bool replace, T value)> update, UInt32 index = 0)
        {
            int count = 0;
            var last = index + 16;
            for (uint i = index; i < last; i++)
            {
                var node = Buffer[i];
                var fi = node.FirstChildIndex;
                if (node.HasValue)
                {
                    var uv = update(node.Key, node.Value);
                    if (uv.replace)
                    {
                        node.Update(node.Key, uv.value);
                        count++;
                    }
                    continue;
                }
                if (!node.HasIndex)
                {
                    continue;
                }
                count += Update(update, fi);
            }
            return count;
        }

        protected override IEnumerable<(string key, T value, UInt32 index)> Enumerate(UInt32 index)
        {
            var last = index + 16;
            for (UInt32 i = index; i < last; i++)
            {
                var node = Buffer[i];
                var fi = node.FirstChildIndex;
                if (node.HasValue)
                {
                    yield return (node.Key, node.Value, i);
                }
                if (!node.HasIndex)
                {
                    continue;
                }
                foreach (var a in Enumerate(fi)) yield return a;
            }
        }

        protected override ref TrieNode GetTrieNode(string key, bool create = false)
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

        private UInt32 GetNode(UInt32 position, bool create = false)
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
            if (!v.HasIndex)
            {
                if (!create)
                {
                    return uint.MaxValue;
                }
                v.UpdateIndex(next);
                next += 16;
                this.EnsureCapacity(v.FirstChildIndex);
            }
            return v.FirstChildIndex;
        }

        void EnsureCapacity(UInt32 i1)
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