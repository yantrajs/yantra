using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.LightWeight
{


    public struct LightWeightStack<T>
    {

        public struct Item {
            private readonly LightWeightStack<T> stack;
            private readonly int index;

            public Item(in LightWeightStack<T> stack, int index)
            {
                this.stack = stack;
                this.index = index;
            }
            public ref T Value => ref stack.storage[index];

            public ref T Parent => ref stack.storage[index - 1];
        }

        public struct StackWalker
        {
            private readonly T[] storage;
            private int index;

            public StackWalker(T[] storage, int index)
            {
                this.storage = storage;
                this.index = index;
            }

            public bool MoveNext()
            {
                index--;
                return index >= 0;
            }

            public ref T Current => ref storage[index];
        }

        public int Count => storage == null ? -1 : length;

        public StackWalker Walker => new StackWalker(storage, length);

        private T[] storage;
        private int length;

        public LightWeightStack(int size)
        {
            storage = new T[size];
            length = 0;
        }

        public LightWeightStack(in LightWeightStack<T> stack)
        {
            storage = new T[stack.storage.Length];
            Array.Copy(stack.storage, storage, storage.Length);
            length = 0;
        }


        public Item Push()
        {
            EnsureCapacity(length);
            return new Item(this,length++);
        }

        public Item Pop()
        {
            return new Item(this, --length);
        }

        public ref T Top => ref storage[length - 1];

        void EnsureCapacity(int length)
        {
            if (length >= storage.Length)
            {
                Array.Resize(ref storage, length + 4);
            }
        }
        public void Dispose()
        {
            storage = null;
            length = 0;
        }

    }
}
