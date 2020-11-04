using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using YantraJS.Core.Storage;

namespace YantraJS.Core
{
    internal enum TrieNodeState : byte
    {
        HasValue = 1,
        HasIndex = 2,
        HasDefaultValue = 4,
        Null = 0xff
    }
    internal class UInt32Trie<T>: BaseMap<uint, T>
    {

        public UInt32Trie(): base(4, 16)
        {
        }


        protected override void Enumerate(UInt32 index, List<(uint Key, T Value)> all)
        {
            var last = index + this.size;
            for (UInt32 i = index; i < last; i++)
            {
                ref var node = ref Buffer[i];
                if (node.HasValue)
                {
                    all.Add((node.Key, node.Value));
                }
            }
            for (UInt32 i = index; i < last; i++)
            {
                ref var node = ref Buffer[i];
                if (!node.HasIndex)
                {
                    continue;
                }
                var fi = node.FirstChildIndex;
                Enumerate(fi, all);
            }
        }

        protected override ref TrieNode GetTrieNode(uint key, bool create = false)
        {
            ref var node = ref TrieNode.Empty;

            // only case for zero...
            if(key == 0)
            {
                return ref Buffer[0];
            }

            UInt32 start = 0xc0000000;
            Int32 i;
            for (i = 30; i >= 0; i -= 2)
            {
                var bk = ((key & start) >> i);
                if (bk == 0)
                {
                    start >>= 2;
                    continue;
                }
                break;
            }
            var last = i;
            start = 0x3;
            uint index = uint.MaxValue;
            uint keyIndex = key;
            // incremenet of two bits...
            for (i = 0; i <= last; i+=2)
            {
                var bk = keyIndex & 0x3;
                if (index == uint.MaxValue)
                {
                    node = ref Buffer[bk];
                    index = bk;
                } else
                {
                    if (!node.HasIndex)
                    {
                        if (!create)
                        {
                            return ref TrieNode.Empty;
                        }

                        var position = next;
                        next += this.size;
                        this.EnsureCapacity(next);
                        Buffer[index].UpdateIndex(position);
                        index = position + bk;
                        node = ref Buffer[index];
                    }
                    else
                    {
                        index = node.FirstChildIndex + bk;
                        node = ref Buffer[index];
                    }
                }
                //start = start << 2;
                keyIndex >>= 2;
            }

            return ref node;
        }

    }
}
