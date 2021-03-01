using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{
    /// <summary>
    /// Difference between List and SparseList, each item is not stored in continuous array,
    /// instead it is stored as an Array of Array. Making growth easier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SparseList<T> : IList<T>
        
    {

        private T[][] pages;
        private readonly int pageSize = 8;
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
                ref var item = ref GetItem(index, out var hasValue, false);
                if (!hasValue)
                {
                    throw new ArgumentOutOfRangeException($"Item does not exist at {index}");
                }
                return item;
            }
            set {
                ref var location = ref GetItem(index, out var hasValue, true);
                location = value;

            }
        }

        public SparseList(int capacity = 0, int pageSize = 8)
        {
            if (pageSize % 8 != 0)
                throw new ArgumentOutOfRangeException($"PageSize should be multiple of 8");
            this.pageSize = pageSize;
            Resize(capacity);
        }

        private static T defaultItem = default(T);

        private ref T GetItem(int index, out bool hasValue, bool create = false)
        {
            if (create)
            {
                Resize(index + 1);
            }
            else
            {
                if (pages == null)
                {
                    hasValue = false;
                    return ref defaultItem;
                }
            }
            int page = index >> 3;
            ref T[] items = ref pages[page];
            if(items == null)
            {
                if (!create)
                {
                    hasValue = false;
                    return ref defaultItem;
                }
                items = new T[pageSize];
            }
            ref T item = ref items[index & 0x07];
            hasValue = true;
            return ref item;
        }

        private void Resize(int capacity)
        {
            if (capacity == 0)
            {
                return;
            }
            int requiredPageLength = (capacity / pageSize) + 1;
            int pageLength = ((requiredPageLength / 4) + 1) * 4;
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
            int index = 0;
            foreach (var page in pages)
            {
                if (page == null)
                {
                    index += pageSize;
                    continue;
                }
                foreach (var value in page)
                {
                    if (index >= length)
                        return -1;
                    if (Equals(item,value))
                        return index;
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
            ref var location = ref GetItem(index, out var hasValue, true);
            location = item;
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
            foreach(var page in pages)
            {
                if (page == null)
                    continue;
                for (int i = 0; i < page.Length; i++)
                {
                    page[i] = defaultItem;
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
            int index = 0;
            bool found = false;
            foreach (var page in pages)
            {
                if (page == null)
                {
                    index += pageSize;
                    continue;
                }
                foreach(var value in page)
                {
                    if (!Equals(value, item))
                    {
                        index++;
                        continue;
                    }
                    found = true;
                    break;
                }
                if (found)
                    break;
            }
            if (!found)
                return false;
            MoveLeft(index);
            this.length--;
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int index = 0;
            if (length == 0)
                yield break;
            foreach(var page in pages)
            {
                if (page == null)
                {
                    index++;
                    continue;
                }
                if (index >= length)
                    yield break;
                foreach (var value in page)
                {
                    if (index >= length)
                        yield break;
                    yield return value;
                    index++;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
