using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Parser
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

        public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SparseList(int capacity = 0, int pageSize = 8)
        {
            if (pageSize % 8 != 0)
                throw new ArgumentOutOfRangeException($"PageSize should be multiple of 8");
            this.pageSize = pageSize;
            Resize(capacity);
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
                Array.Resize(ref pages, pageLength);
            }
        }

        public int IndexOf(T item)
        {
            int index = 0;
            foreach (var page in pages)
            {
                foreach (var value in page)
                {
                    if (item.Equals(value))
                        return index;
                    index++;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach(var page in pages)
            {
                foreach(var value in page)
                {

                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
