using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("WebAtoms.CoreJS.Tests")]
namespace WebAtoms.CoreJS.Core {

    internal class BinaryCharMap<T>
    {

        private TrieNode<T>[] Buffer;

        private uint next = 16;

        private uint grow = 64;

        private static UInt16 first = (UInt16)0xF000;
        private static UInt16 second = (UInt16)0xF00;
        private static UInt16 third = (UInt16)0xF0;
        private static UInt16 fourth = (UInt16)0xF;

        public BinaryCharMap()
        {
            Buffer = new TrieNode<T>[grow];
        }

        //public IEnumerable<KeyValuePair<string, T>> AllValues()
        //{
        //    uint index = 0;
        //    ushort ch = 0;
        //    while (true)
        //    {
        //        if (Buffer[index].State == State.None)
        //        {
        //            break;
        //        }


        //    }
        //}

        public T this[IEnumerable<char> input]
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

        public bool TryGetValue(IEnumerable<char> key, out T value)
        {
            ref var node = ref GetTrieNode(key);
            if (node.State == State.HasValue)
            {
                value = node.Value;
                return true;
            }
            value = default;
            return false;
        }

        public bool Remove(IEnumerable<char> key)
        {
            ref var node = ref GetTrieNode(key, false);
            if (node.State == State.HasValue)
            {
                node.State = State.Initialized;
                node.Value = default;
                return true;
            }
            return false;
        }

        private void Save(IEnumerable<char> key, T value)
        {
            ref var node = ref GetTrieNode(key, true);
            if (node.IsEmpty)
            {
                throw new InvalidOperationException();
            }
            node.Value = value;
            node.State = State.HasValue;
        }

        private ref TrieNode<T> GetTrieNode(IEnumerable<char> key, bool create = false)
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
                    return ref TrieNode<T>.Empty;
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
            if (v.State == State.None)
            {
                if (!create)
                {
                    return uint.MaxValue;
                }
                // initailize..
                v.State = State.Initialized;
                v.FirstChildIndex = next;
                next += 4;
                this.EnsureCapacity(v.FirstChildIndex);
            }
            return v.FirstChildIndex;
            // return ref Buffer[v.FirstChildIndex];
            // return v.FirstChildIndex;
        }

        private uint SaveNode(uint position)
        {
            this.EnsureCapacity(position);
            var v1 = Buffer[position];
            if (v1.FirstChildIndex == 0)
            {
                if (v1.State == State.None)
                {
                    // initailize..
                    v1.State = State.Initialized;
                    v1.FirstChildIndex = next;
                    next += 16;
                    Buffer[position] = v1;
                }
            }
            return v1.FirstChildIndex;
        }


        void EnsureCapacity(uint i1)
        {
            if (this.Buffer.Length <= i1)
            {
                // add 16  more...
                var b = new TrieNode<T>[i1 + grow];
                Array.Copy(this.Buffer, b, this.Buffer.Length);
                this.Buffer = b;
            }
        }
    }

    internal enum State: byte
    {
        None = 0,
        Initialized = 1,
        HasValue = 2
    }

    internal struct TrieNode<T>
    {

        internal static TrieNode<T> Empty = new TrieNode<T> {
            FirstChildIndex = uint.MaxValue
        };

        internal bool IsEmpty => FirstChildIndex == uint.MaxValue;

        internal State State;

        /// <summary>
        /// Index to next node set...
        /// </summary>
        internal UInt32 FirstChildIndex;

        /// <summary>
        /// Value of the current node
        /// </summary>
        internal T Value;
    }    

}