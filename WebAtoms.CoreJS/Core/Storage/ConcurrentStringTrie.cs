using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WebAtoms.CoreJS.Tests")]
namespace WebAtoms.CoreJS.Core
{
    internal class ConcurrentStringTrie<T> : ConcurrentTrie<string, T>
    {

        public ConcurrentStringTrie() : base(4, 1024)
        {

        }

        public ConcurrentStringTrie(uint size) : base(4, size)
        {

        }


        int bitSize = 2;
        uint bitBig = 0xC000;
        uint bitLast = 0x3;
        int bitLength = 14;

        public int Total = 0;

        public int Size => Buffer.Length;

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
                                // this.Save(old, dirty);
                                // node.Update(old, dirty);

                                // need to save without lock...
                                ref var newNode = ref GetTrieNode(old, true);
                                newNode.Update(old, dirty);

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
    }
}