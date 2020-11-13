using System;
using System.Security.Cryptography.X509Certificates;

namespace YantraJS.Core
{
    //internal class CompactUInt32Trie<T> : UInt32Trie<T>
    //{
    //    protected override ref TrieNode GetTrieNode(uint key, bool create = false)
    //    {
    //        ref var node = ref TrieNode.Empty;

    //        // only case for zero...
    //        if (key == 0)
    //        {
    //            node = ref Buffer[0];
    //            if (node.Key != 0)
    //            {
    //                // move...
    //                var old = node.Key;
    //                var dirty = node.Value;
    //                node.UpdateDefaultValue(0, default);
    //                Save(old, dirty);
    //            }
    //            return ref Buffer[0];
    //        }

    //        Int32 i;
    //        uint index = uint.MaxValue;
    //        uint keyIndex = key;
    //        // incremenet of two bits...
    //        for (i = 0; i < 32; i += 2)
    //        {
    //            var bk = keyIndex & 0x3;
    //            if (index == uint.MaxValue)
    //            {
    //                node = ref Buffer[bk];
    //                index = bk;
    //            }
    //            else
    //            {
    //                if (!node.HasIndex)
    //                {
    //                    if (!create)
    //                    {
    //                        return ref TrieNode.Empty;
    //                    }

    //                    var position = next;
    //                    next += this.size;
    //                    this.EnsureCapacity(next);
    //                    Buffer[index].UpdateIndex(position);
    //                    index = position + bk;
    //                    node = ref Buffer[index];
    //                    return ref node;
    //                }
    //                else
    //                {
    //                    index = node.FirstChildIndex + bk;
    //                    node = ref Buffer[index];
    //                }

    //            }

    //            if (node.Key == key)
    //            {
    //                return ref node;
    //            }
    //            if (create)
    //            {
    //                if (!(node.HasValue || node.HasDefaultValue))
    //                {
    //                    return ref node;
    //                }
    //                if (node.Key > key)
    //                {
    //                    var dirty = node.Value;
    //                    var old = node.Key;
    //                    node.UpdateDefaultValue(key, default);
    //                    this.Save(old, dirty);
    //                    return ref Buffer[index];
    //                }
    //            }


    //            // start <<= 2;
    //            keyIndex >>= 2;
    //        }
    //        if (create)
    //        {
    //            if (node.Key > key)
    //            {
    //                var dirty = node.Value;
    //                var old = node.Key;
    //                node.UpdateDefaultValue(key, default);
    //                this.Save(old, dirty);
    //                return ref Buffer[index];
    //            }
    //        }
    //        return ref node;
    //    }
    //}
}
