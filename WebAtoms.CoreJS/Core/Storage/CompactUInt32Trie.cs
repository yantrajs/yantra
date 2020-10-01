using System;

namespace WebAtoms.CoreJS.Core
{
    internal class CompactUInt32Trie<T> : UInt32Trie<T>
    {
        //protected override ref TrieNode GetTrieNode(uint key, bool create = false)
        //{
        //    ref var node = ref TrieNode.Empty;

        //    // only case for zero...
        //    if (key == 0)
        //    {
        //        return ref Buffer[0];
        //    }

        //    UInt32 start = 0xc0000000;
        //    Int32 i;
        //    for (i = 30; i >= 0; i -= 2)
        //    {
        //        var bk = ((key & start) >> i);
        //        if (bk == 0)
        //        {
        //            start >>= 2;
        //            continue;
        //        }
        //        break;
        //    }
        //    var last = i;
        //    start = 0x3;
        //    uint index = uint.MaxValue;
        //    // incremenet of two bits...
        //    for (i = 0; i <= last; i += 2)
        //    {
        //        var bk = ((key & start) >> i);
        //        if (index == uint.MaxValue)
        //        {
        //            node = ref Buffer[bk];
        //            index = bk;
        //            if (node.Key == key)
        //            {
        //                return ref node;
        //            }
        //            if (create)
        //            {
        //                if (!(node.HasValue | node.HasDefaultValue))
        //                {
        //                    return ref node;
        //                }
        //                if (node.Key > key)
        //                {
        //                    var dirty = node.Value;
        //                    var old = node.Key;
        //                    node.UpdateDefaultValue(key, default);
        //                    this.Save(old, dirty);
        //                    return ref node;
        //                }
        //            }

        //        }
        //        else
        //        {
        //            if (node.Key == key)
        //            {
        //                return ref node;
        //            }

        //            if (create)
        //            {
        //                if (!(node.HasValue | node.HasDefaultValue))
        //                {
        //                    return ref node;
        //                }
        //                if (node.Key > key)
        //                {
        //                    var dirty = node.Value;
        //                    var old = node.Key;
        //                    node.UpdateDefaultValue(key, default);
        //                    this.Save(old, dirty);
        //                    return ref node;
        //                }
        //            }

        //            if (!node.HasIndex)
        //            {
        //                if (!create)
        //                {
        //                    return ref TrieNode.Empty;
        //                }

        //                var position = next;
        //                next += this.size;
        //                this.EnsureCapacity(next);
        //                Buffer[index].UpdateIndex(position);
        //                index = position + bk;
        //                node = ref Buffer[index];
        //            }
        //            else
        //            {
        //                index = node.FirstChildIndex + bk;
        //                node = ref Buffer[index];
        //            }

        //        }
        //        start <<= 2;
        //    }
        //    if (node.Key > key)
        //    {
        //        var dirty = node.Value;
        //        var old = node.Key;
        //        node.UpdateDefaultValue(key, default);
        //        this.Save(old, dirty);
        //        return ref node;
        //    }
        //    return ref node;
        //}
    }
}
