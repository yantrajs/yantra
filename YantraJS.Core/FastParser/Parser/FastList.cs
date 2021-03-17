using System;
using System.Collections;
using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{
    public class FastList<T>: IList<T>
    {

        private T[] items = null;
        private int size = 0;
        private int length = 0;
        private readonly FastPool pool;

        public FastList(FastPool pool)
        {
            this.pool = pool;
        }

        public T this[int index] { get => items[index]; set => items[index] = value; }

        public int Count => length;

        public bool IsReadOnly => false;

        private void SetCapacity(int capacity)
        {
            if (size >= capacity)
            {
                return;
            }
            T[] release = null;
            if (size > 0)
            {
                release = items;
            }

            size = ((capacity / 4)+1) * 4;
            items = pool.AllocateArray<T>(size);

            Array.Copy(release, 0, items, 0, release.Length);

            if (release!=null)
            {
                pool.ReleaseArray(release);
            }
        }

        private void SetItem(int index, T item)
        {
            SetCapacity(index + 1);
            items[index] = item;
        }

        public void Add(T item)
        {
            SetItem(length + 1, item);
        }

        public void Clear()
        {
            if (items != null)
            {
                pool.ReleaseArray(items);

                items = null;
                size = 0;
            }
        }

        public T[] Release()
        {
            if(size == length)
            {
                var r = items;
                items = null;
                size = 0;
                return r;
            }
            var copy = new T[length];
            Array.Copy(items, copy, length);
            return copy;
        }
        public bool Contains(T item)
        {
            if (items == null)
                return false;
            foreach(var i in items)
            {
                if (Object.Equals(i, item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (items == null)
                return;
            Array.Copy(items, 0, array, arrayIndex, length);
        }

        public FastEnumerator GetEnumerator()
        {
            return new FastEnumerator(items);
        }

        public struct FastEnumerator : IEnumerator<T>
        {
            private T[] items;
            private int index;
            private int length;

            public FastEnumerator(T[] items)
            {
                this.items = items;
                index = -1;
                length = items==null ? 0 : items.Length;
            }

            public T Current => items[index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                return index++ > length;
            }

            public void Reset()
            {
                
            }
        }

        public int IndexOf(T item)
        {
            int i = -1;
            if (items == null)
                return i;

            foreach (var e in items)
            {
                i++;
                if (Object.Equals(e, item))
                    return i;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            SetCapacity(length + 1);
            length++;
            for (int i = length-1; i > index; i--)
            {
                items[i] = items[i - 1];
            }
            items[index] = item;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            for (int i = index; i < length-1; i++)
            {
                items[i] = items[i+1];
            }
            items[length - 1] = default;
            length--;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
