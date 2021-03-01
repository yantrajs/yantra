using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        public struct Page
        {
            private T Item0;
            private T Item1;
            private T Item2;
            private T Item3;
            private T Item4;
            private T Item5;
            private T Item6;
            private T Item7;

            public T this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return Item0;
                        case 1: return Item1;
                        case 2: return Item2;
                        case 3: return Item3;
                        case 4: return Item4;
                        case 5: return Item5;
                        case 6: return Item6;
                        case 7: return Item7;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0: 
                            Item0 = value;
                            break;
                        case 1:
                            Item1 = value;
                            break;
                        case 2:
                            Item2 = value;
                            break;
                        case 3:
                            Item3 = value;
                            break;
                        case 4:
                            Item4 = value;
                            break;
                        case 5:
                            Item5 = value;
                            break;
                        case 6:
                            Item6 = value;
                            break;
                        case 7:
                            Item7 = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }

                }
            }

            internal void Clear()
            {
                Item0 = default;
                Item1 = default;
                Item2 = default;
                Item3 = default;
                Item4 = default;
                Item5 = default;
                Item6 = default;
                Item7 = default;
            }
        }


        private Page[] pages;
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
                SetValue(index, value);
            }
        }

        public SparseList(int capacity = 0)
        {
            Resize(capacity);
        }

        private static Page defaultItem;

        private bool TryGetValue(int index, out T value)
        {
            if (index >= length)
            {
                value = default;
                return false;
            }
            ref var Page = ref GetPage(index, out var hasValue, false);
            if (!hasValue) {
                value = default;
                return false;
            }
            value = Page[index & 0x7];
            return true;
        }

        private void SetValue(int index,in T item)
        {
            if(index >= length)
            {
                throw new IndexOutOfRangeException();
            }
            ref var page = ref GetPage(index, out var hasValue, true);
            page[index & 0x7] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref Page GetPage(int index, out bool hasValue, bool create = false)
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
            ref Page items = ref pages[page];
            hasValue = true;
            return ref items;
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
                pages = new Page[pageLength];
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
            ref var location = ref GetPage(index, out var hasValue, true);
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
                ref var page = ref pages[i];
                page.Clear();
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

        public IEnumerator<T> GetEnumerator()
        {
            if (length == 0 || pages == null)
                yield break;
            for (int i = 0; i < length; i++)
            {
                if (TryGetValue(i, out var item))
                    yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
