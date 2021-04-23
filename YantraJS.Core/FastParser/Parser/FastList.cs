using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace YantraJS.Core.FastParser
{
    public class FastList<T>: IList<T>, IDisposable
    {

        private T[] items = null;
        private int size = 0;
        private int length = 0;
        private readonly FastPool pool;

        public FastList(FastPool pool)
        {
            this.pool = pool;
        }

        public FastList(FastPool pool, int size)
        {
            this.pool = pool;
            SetCapacity(size);
        }


        public T this[int index] { get => items[index]; set => items[index] = value; }

        public int Count => length;

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            size = capacity;
            items = pool.AllocateArray<T>(size);
            size = items.Length;
            if (release!=null)
            {
                Array.Copy(release, 0, items, 0, release.Length);
                pool.ReleaseArray(release);
            }
        }

        public void Add(T item)
        {
            var i = length++;
            SetCapacity(length);
            items[i] = item;
        }

        public bool Any() => length > 0;
        public void Clear()
        {
            if (items != null)
            {
                pool.ReleaseArray(items);

                items = null;
                length = 0;
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
                length = 0;
                return r;
            }
            var copy = new T[length];
            Array.Copy(items, copy, length);
            return copy;
        }

        public ArraySpan<T> ToSpan()
        {
            var array = items;
            var length = this.length;
            var a = new ArraySpan<T>(array, length);
            items = null;
            this.length = 0;
            this.size = 0;
            return a;
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
            return new FastEnumerator(this);
        }

        public ReverseEnumerator GetReverseEnumerator() => new ReverseEnumerator(this);

        public struct ReverseEnumerator
        {
            private readonly T[] items;
            private int index;

            public ReverseEnumerator(FastList<T> list)
            {
                this.items = list.items;
                this.index = list.length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext(out T item)
            {
                if(--index >= 0)
                {
                    item = items[index];
                    return true;
                }
                item = default;
                return false;
            }
        }

        public struct FastEnumerator : IEnumerator<T>
        {
            private readonly T[] items;
            private int index;
            private readonly int length;

            public FastEnumerator(FastList<T> list)
            {
                this.items = list.items;
                index = -1;
                length = list.length;
            }

            public T Current => items[index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                return ++index < length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext(out T item) {
                if(++index < length) {
                    item = items[index];
                    return true;
                }
                item = default;
                return false;
            }

            public void Reset()
            {
                
            }
        }

        internal void AddRange(FastList<T> initList)
        {
            var e = initList.GetEnumerator();
            while (e.MoveNext(out var item))
                Add(item);
        }


        internal void AddRange(IEnumerable<T> initList)
        {
            foreach (var exp in initList)
                Add(exp);
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

        public void Dispose()
        {
            Clear();
        }
    }
}
