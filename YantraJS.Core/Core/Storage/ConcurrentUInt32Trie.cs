using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using YantraJS.Core.Storage;

namespace YantraJS.Core
{
    //internal class ConcurrentUInt64Trie<T> : ConcurrentTrie<ulong, T>
    //{

    //    public ConcurrentUInt64Trie() : base(4, 8)
    //    {

    //    }

    //    protected override ref TrieNode GetTrieNode(ulong key, bool create = false)
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
    //                // Save(old, dirty);
    //                ref var node1 = ref GetTrieNode(old, true);
    //                node1.Update(old, dirty);
    //            }
    //            return ref Buffer[0];
    //        }

    //        Int32 i;
    //        ulong index = ulong.MaxValue;
    //        ulong keyIndex = key;
    //        // incremenet of two bits...
    //        for (i = 0; i < 64; i += 2)
    //        {
    //            var bk = keyIndex & 0x3;
    //            if (index == ulong.MaxValue)
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
    //                    // this.Save(old, dirty);
    //                    ref var node1 = ref GetTrieNode(old, true);
    //                    node1.Update(old, dirty);

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
    //                // this.Save(old, dirty);
    //                ref var node1 = ref GetTrieNode(old, true);
    //                node1.Update(old, dirty);

    //                return ref Buffer[index];
    //            }
    //        }
    //        return ref node;
    //    }

    //}



    //internal class ConcurrentUInt32Trie<T>: ConcurrentTrie<uint, T>
    //{

    //    public ConcurrentUInt32Trie(): base(4,8)
    //    {

    //    }

    //    #region GetTrieNode
    //    //protected override ref TrieNode GetTrieNode(uint key, bool create = false)
    //    //{
    //    //    ref var node = ref TrieNode.Empty;

    //    //    // only case for zero...
    //    //    if (key == 0)
    //    //    {
    //    //        return ref Buffer[0];
    //    //    }

    //    //    UInt32 start = 0xc0000000;
    //    //    Int32 i;
    //    //    for (i = 30; i >= 0; i -= 2)
    //    //    {
    //    //        var bk = ((key & start) >> i);
    //    //        if (bk == 0)
    //    //        {
    //    //            start >>= 2;
    //    //            continue;
    //    //        }
    //    //        break;
    //    //    }
    //    //    var last = i;
    //    //    start = 0x3;
    //    //    uint index = uint.MaxValue;
    //    //    uint keyIndex = key;
    //    //    // incremenet of two bits...
    //    //    for (i = 0; i <= last; i += 2)
    //    //    {
    //    //        var bk = keyIndex & 0x3;
    //    //        if (index == uint.MaxValue)
    //    //        {
    //    //            node = ref Buffer[bk];
    //    //            index = bk;
    //    //        }
    //    //        else
    //    //        {
    //    //            if (!node.HasIndex)
    //    //            {
    //    //                if (!create)
    //    //                {
    //    //                    return ref TrieNode.Empty;
    //    //                }

    //    //                var position = next;
    //    //                next += this.size;
    //    //                this.EnsureCapacity(next);
    //    //                Buffer[index].UpdateIndex(position);
    //    //                index = position + bk;
    //    //                node = ref Buffer[index];
    //    //            }
    //    //            else
    //    //            {
    //    //                index = node.FirstChildIndex + bk;
    //    //                node = ref Buffer[index];
    //    //            }
    //    //        }
    //    //        //start = start << 2;
    //    //        keyIndex >>= 2;
    //    //    }

    //    //    return ref node;
    //    //}
    //    #endregion

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
    //                // Save(old, dirty);
    //                ref var node1 = ref GetTrieNode(old, true);
    //                node1.Update(old, dirty);
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
    //                    // this.Save(old, dirty);
    //                    ref var node1 = ref GetTrieNode(old, true);
    //                    node1.Update(old, dirty);

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
    //                // this.Save(old, dirty);
    //                ref var node1 = ref GetTrieNode(old, true);
    //                node1.Update(old, dirty);

    //                return ref Buffer[index];
    //            }
    //        }
    //        return ref node;
    //    }

    //}
}
