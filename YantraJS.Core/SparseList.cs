using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core
{
    public class SparseStack<T>
    {
        private SparseList<T> list = new SparseList<T>();

        public void Push(T item)
        {
            list.Add(item);
        }

        public int Count => list.Count;

        public T Pop()
        {
            if (list.Count == 0)
                throw new IndexOutOfRangeException();
            return list.RemoveLast();
        }

        public T Peek => list[list.Count - 1];
    }

    /// <summary>
    /// Difference between List and SparseList, each item is not stored in continuous array,
    /// instead it is stored as an Array of Array. Making growth easier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SparseList<T> : IList<T> {

        private T[][] pages;
        private const int pageSize = 8;
        private int length;

        public int Count => length;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get {
                if (index >= length)
                {
                    throw new ArgumentOutOfRangeException($"Item does not exist at {index}");
                }
                if (!TryGetValue(index, out var item))
                {
                    throw new ArgumentOutOfRangeException($"Item does not exist at {index}");
                }
                return item;
            }
            set {
                SetValue(index, in value);
            }
        }

        public SparseList(int capacity = 0)
        {
            Resize(capacity);
        }

        // private static Page defaultItem;

        private bool TryGetValue(int index, out T value)
        {
            if (index >= length)
            {
                value = default;
                return false;
            }
            var page = GetPage(index, false);
            if (page == null) {
                value = default;
                return false;
            }
            value = page[index & 0x7];
            return true;
        }

        private void SetValue(int index,in T item)
        {
            if(index >= length)
            {
                throw new IndexOutOfRangeException();
            }
            var page = GetPage(index, true);
            page[index & 0x7] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T[] GetPage(int index, bool create = false)
        {
            if (create)
            {
                Resize(index + 1);
            }
            else
            {
                if (pages == null)
                {
                    return null;
                }
            }
            int page = index >> 3;
            var items = pages[page];
            if(items == null && create)
            {
                return pages[page] = new T[pageSize];
            }
            return items;
        }

        private void Resize(int capacity)
        {
            if (capacity == 0)
            {
                return;
            }

            capacity = ((capacity >> 5) + 1) << 5;

            // int requiredPageLength = (capacity / pageSize) + 1;
            // int pageLength = ((requiredPageLength / 4) + 1) * 4;
            int pageLength = capacity / pageSize;
            if(pages == null)
            {
                pages = new T[pageLength][];
            } else
            {
                if (pageLength <= pages.Length)
                    return;
                Array.Resize(ref pages, pageLength);
            }
        }

        public int IndexOf(T item)
        {
            if (pages == null)
                return -1;
            for (int i = 0; i < pages.Length; i++)
            {
                ref var page = ref pages[i];
                var index = i << 3;
                for (int j = 0; j < 7; j++)
                {
                    if (index >= length)
                        return -1;
                    if (Equals(page[j],item)) {
                        return index;
                    }
                    index++;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index >= length)
            {
                length = index + 1;
            } else
            {
                length++;
                // move..
                MoveRight(index);
            }
            var location = GetPage(index, true);
            location[index & 7] = item;
        }

        private void MoveRight(int index)
        {
            for (int i = length - 1 ; i > index; i--)
            {
                this[i] = this[i-1];
            }
        }

        private void MoveLeft(int index)
        {
            for (int i = index; i < length - 1; i++)
            {
                this[i] = this[i + 1];
            }
            this[length - 1] = default;
        }


        public void RemoveAt(int index)
        {
            // move items up...
            if (index >= length)
                throw new ArgumentOutOfRangeException();
            MoveLeft(index);
            this.length--;
        }

        public T RemoveLast()
        {
            var index = length - 1;
            var page = GetPage(index);
            var item = page[index & 0x7];
            page[index & 0x7] = default;
            this.length--;
            return item;
        }

        public void Add(T item)
        {
            Insert(length, item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach(var item in items)
            {
                Insert(length, item);
            }
        }

        public void Clear()
        {
            length = 0;
            if (pages == null)
                return;
            for (int i = 0; i < pages.Length; i++)
            {
                var page = pages[i];
                for (int j = 0; j < pageSize; j++)
                {
                    page[j] = default;
                }
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < length)
                throw new ArgumentOutOfRangeException();
            var index = arrayIndex;
            foreach(var item in this)
            {
                array[index++] = item;
            }
        }

        public bool Remove(T item)
        {
            if (pages == null)
                return false;
            int index = IndexOf(item);
            if (index == -1)
                return false;
            MoveLeft(index);
            this.length--;
            return true;
        }

        public SparseEnumerator GetEnumerator()
        {
            return new SparseEnumerator(this.pages, this.length);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct SparseEnumerator : IEnumerator<T>
        {
            private readonly T[][] pages;
            private readonly int length;
            private int index;

            public SparseEnumerator(T[][] pages, int length)
            {
                this.pages = pages;
                this.length = length;
                index = -1;
            }

            public T Current
            {
                get
                {
                    var page = index >> 3;
                    return pages[page][index & 0x7];
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                return (++index) < length;
            }

            public void Reset()
            {
                
            }
        }
    }
}
