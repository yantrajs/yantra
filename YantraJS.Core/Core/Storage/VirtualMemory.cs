using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Core.Storage
{
    public class VirtualMemory<T>
    {

        private T[] nodes = null;
        private int last = 0;

        private int count = 0;

        public void Allocate(int length)
        {
            if (this.count <= length)
            {
                // we need to resize...
                var max = this.count * 2;
                if (this.count * 2 > length)
                {
                    length = this.count * 2;
                }
                this.SetCapacity(length);
            }
        }


    }

    public readonly struct VirtualSpan<T>
    {
        private readonly T[] nodes;
        private readonly int offset;
        public readonly int Length;

        public ref T this[int index]
        {
            get
            {
                return ref nodes[offset + index];
            }
        }
    }
}
