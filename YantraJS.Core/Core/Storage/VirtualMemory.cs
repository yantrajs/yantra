﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Core.Storage
{
    public struct VirtualMemory<T>
    {

        private T[] nodes = null;
        private int last = 0;

        public bool IsEmpty => this.nodes == null;

        public VirtualMemory()
        {
            
        }

        public ref T this[VirtualArray a, int index]
        {
            get
            {
                return ref this.nodes[a.Offset + index];
            }
        }

        public VirtualArray Allocate(int length)
        {
            var max = this.last + length;
            if (this.nodes == null || this.nodes.Length <= length)
            {
                // we need to resize...
                var capacity = this.last * 2;
                if (capacity <= length)
                {
                    capacity = ((length / 16)+ 1) * 16;
                }
                this.SetCapacity(capacity);
            }
            var offset = this.last;
            this.last += length;
            return new VirtualArray(offset, length);
        }

        public void SetCapacity(int max)
        {
            if (max <=0)
            {
                return;
            }
            if (nodes == null)
            {
                nodes = new T[max];
                return;
            }

            if (this.nodes.Length >= max)
            {
                return;
            }
            System.Array.Resize(ref this.nodes, max);
        }
    }

    public readonly struct VirtualArray
    {
        public readonly int Offset;
        public readonly int Length;

        public VirtualArray()
        {
            this.Offset = -1;
        }

        public VirtualArray(int offset, int length)
        {
            this.Offset = offset;
            this.Length = length;
        }

        public bool IsEmpty => this.Length == 0;
    }
}
