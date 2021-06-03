using System;

namespace YantraJS.Core.FastParser
{
    public readonly struct FastStack<T>
    {
        private readonly FastList<T> list;

        public FastStack(FastPool pool)
        {
            list = new FastList<T>(pool);
        }

        public void Push(T item)
        {
            list.Add(item);
        }

        public T Pop()
        {
            var last = list.Count-1;
            if (last < 0)
                throw new IndexOutOfRangeException();
            var item = list[last];
            list.RemoveAt(last);
            return item;
        }

        public T Peek => list[list.Count - 1];

        public bool IsEmpty => list.Count == 0;

        public int Count => list.Count;

        public void Clear()
        {
            list.Clear();
        }
    }
}
