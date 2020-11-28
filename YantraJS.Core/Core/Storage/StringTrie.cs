using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using YantraJS.Core.Storage;

namespace YantraJS.Core {

    //internal class CharMapValue<T>
    //{
    //    public string Key;
    //    public T Value;
    //}

    //internal class StringTrie<T>: BaseMap<string, T>
    //{

    //    public StringTrie(): base(4, 1024)
    //    {

    //    }

    //    const int bitSize = 2;
    //    const uint bitBig = 0xC000;
    //    const uint bitLast = 0x3;
    //    const int bitLength = 14;

    //    public int Total = 0;

    //    public int Size => Buffer.Length;        

    //    protected override void Enumerate(UInt32 index, List<(string Key, T Value)> all)
    //    {
    //        var last = index + this.size;
    //        for (UInt32 i = index; i < last; i++)
    //        {
    //            ref var node = ref Buffer[i];
    //            if (node.HasValue)
    //            {
    //                all.Add((node.Key, node.Value));
    //            }
    //            if (!node.HasIndex)
    //            {
    //                continue;
    //            }
    //            Enumerate(node.FirstChildIndex, all);
    //        }
    //    }

    //    protected override ref TrieNode GetTrieNode(string keyString, bool create = false)
    //    {
    //        ref var node = ref TrieNode.Empty;

    //        bool created = false;



    //        uint index = uint.MaxValue;
    //        foreach (var ch in keyString)
    //        {
    //            UInt16 key = ch;
    //            uint start = bitBig;
    //            Int32 i;
    //            for (i = bitLength; i >= 0; i -= bitSize)
    //            {
    //                byte bk = (byte)((key & start) >> i);
    //                if (bk == 0)
    //                {
    //                    start >>= bitSize;
    //                    continue;
    //                }
    //                break;
    //            }
    //            var last = i;
    //            start = bitLast;
    //            // incremenet of two bits...
    //            uint keyIndex = key;
    //            for (i = 0; i <= last; i += bitSize)
    //            {
    //                byte bk = (byte)(keyIndex & bitLast);
    //                if (index == uint.MaxValue)
    //                {
    //                    node = ref Buffer[bk];
    //                    index = bk;
    //                }
    //                else
    //                {
    //                    if (node.Key == keyString)
    //                        return ref node;
    //                    // if this branch has no value
    //                    // store value here...
    //                    if (create)
    //                    {
    //                        if (!(node.HasValue | node.HasDefaultValue))
    //                        {
    //                            return ref node;
    //                        }
    //                        if (node.Key.CompareTo(keyString) > 0)
    //                        {
    //                            var dirty = node.Value;
    //                            var old = node.Key;
    //                            node.UpdateDefaultValue(keyString, default);
    //                            this.Save(old, dirty);
    //                            return ref Buffer[index];
    //                        }
    //                    }


    //                    if (!node.HasIndex)
    //                    {
    //                        if (!create)
    //                        {
    //                            return ref TrieNode.Empty;
    //                        }
    //                        created = true;
    //                        var position = next;
    //                        next += this.size;
    //                        this.EnsureCapacity(next);
    //                        Buffer[index].UpdateIndex(position);
    //                        index = position + bk;
    //                        node = ref Buffer[index];
    //                        return ref node;
    //                    }
    //                    else
    //                    {
    //                        index = node.FirstChildIndex + bk;
    //                        node = ref Buffer[index];
    //                    }
    //                }
    //                keyIndex >>= bitSize;
    //            }
    //        }

    //        if ((node.HasValue | node.HasDefaultValue) && node.Key.CompareTo(keyString) > 0)
    //        {
    //            var dirty = node.Value;
    //            var old = node.Key;
    //            node.UpdateDefaultValue(keyString, default);
    //            this.Save(old, dirty);
    //            return ref Buffer[index];
    //        }

    //        if (created)
    //        {
    //            Total += keyString.Length;
    //        }
    //        return ref node;
    //    }
    //}
}