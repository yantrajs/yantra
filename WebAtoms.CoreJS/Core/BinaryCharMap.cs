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

        private uint next = 4;
        private uint size = 4;
        private uint grow = 1024;
        int bitSize = 2;
        uint bitBig = 0xC000;
        uint bitLast = 0x3;
        int bitLength = 14;

        //private uint next = 2;
        //private uint size = 2;
        //private uint grow = 1024;
        //int bitSize = 1;
        //uint bitBig = 0x8000;
        //uint bitLast = 0x1;
        //int bitLength = 15;

        //private uint next = 16;
        //private uint size = 16;
        //private uint grow = 1024;
        //int bitSize = 4;
        //uint bitBig = 0xF000;
        //uint bitLast = 0xF;
        //int bitLength = 12;


        public int Total = 0;

        public int Size => Buffer.Length;

        public BinaryCharMap()
        {
            Buffer = new TrieNode[grow];
        }

        public override int Update(Func<string, T, (bool replace, T value)> update, UInt32 index = 0)
        {
            int count = 0;
            var last = index + this.size;
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
            var last = index + this.size;
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

        protected override ref TrieNode GetTrieNode(string keyString, bool create = false)
        {
            ref var node = ref TrieNode.Empty;

            bool created = false;



            uint index = uint.MaxValue;
            foreach (var ch in keyString)
            {
                UInt16 key = ch;
                uint start = bitBig;
                Int32 i;
                for (i = bitLength; i >= 0; i -= bitSize)
                {
                    byte bk = (byte)((key & start) >> i);
                    if (bk == 0)
                    {
                        start = start >> bitSize;
                        continue;
                    }
                    break;
                }
                var last = i;
                start = bitLast;
                // incremenet of two bits...
                for (i = 0; i <= last; i += bitSize)
                {
                    byte bk = (byte)((key & start) >> i);
                    if (index == uint.MaxValue)
                    {
                        node = ref Buffer[bk];
                        index = bk;
                    }
                    else
                    {
                        if (node.HasValue && node.Key == keyString)
                            return ref node;
                        // if this branch has no value
                        // store value here...
                        if (create)
                        {
                            if (!(node.HasValue | node.HasDefaultValue))
                            {
                                return ref node;
                            }
                            if (node.Key.CompareTo(keyString) > 0)
                            {
                                var dirty = node.Value;
                                var old = node.Key;
                                node.UpdateDefaultValue(keyString, default);
                                this.Save(old, dirty);
                                return ref node;
                            }
                        }


                        if (!node.HasIndex)
                        {
                            if (!create)
                            {
                                return ref TrieNode.Empty;
                            }
                            created = true;
                            var position = next;
                            next += this.size;
                            this.EnsureCapacity(next);
                            Buffer[index].UpdateIndex(position);
                            index = position + bk;
                            node = ref Buffer[index];
                            return ref node;
                        }
                        else
                        {
                            index = node.FirstChildIndex + bk;
                            node = ref Buffer[index];
                        }
                    }
                    start = start << bitSize;
                }
            }

            if (created)
            {
                Total += keyString.Length;
            }
            return ref node;
        }

        void EnsureCapacity(UInt32 i1)
        {
            if (this.Buffer.Length <= i1)
            {
                // add 16  more...
                var b = new TrieNode[i1 + grow];
                Array.Copy(this.Buffer, b, this.Buffer.Length);
                Console.WriteLine($"Allocated {b.Length}");
                this.Buffer = b;
            }
        }
    }

}