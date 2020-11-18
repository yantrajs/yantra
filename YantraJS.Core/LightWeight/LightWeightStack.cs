using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.LightWeight
{


    public struct LightWeightStack<T>
    {

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
                this.index--;
                return this.index >= 0;
            }

            public ref T Current => ref storage[this.index];
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
            length = stack.length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Push()
        {
            EnsureCapacity(length);
            var x = Activator.CreateInstance<T>();
            storage[length++] = x;
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pop()
        {
            --length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetAt(int index)
        {
            return ref storage[index];
        }

        public ref T Top => ref storage[length];

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
