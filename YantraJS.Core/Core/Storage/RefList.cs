using System;

namespace YantraJS.Core
{
    public struct RefList<T>
    {
        const int Size = 16;

        private T[] array;
        private int last;
        public int Length => last;

        public ref T Put()
        {
            if (array == null)
            {
                array = new T[Size];
            } else if (last == array.Length) 
            {
                Array.Resize(ref array, array.Length + Size);
            }
            return ref array[last++];
        }

        public void Remove(T item)
        {
            var index = Array.IndexOf(array, item);
            if (index == -1)
            {
                return;
            }
            while(index < last - 1)
            {
                array[last] = array[last + 1];
            }
            last--;
        }
    }
}
