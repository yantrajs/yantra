using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{
    public  interface IFastEnumerable<T>
    {
        FastEnumerator<T> GetFastEnumerator();
    }

    public abstract class FastEnumerator<T>: IEnumerator<T>
    {
        public T Current { get; set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            
        }

        public abstract bool MoveNext(out T item);

        public bool MoveNext()
        {
            if(MoveNext(out var item))
            {
                Current = item;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class Sequence<T>: IReadOnlyList<T>, IFastEnumerable<T>
    {

        private T[] items;
        private Sequence<T> next;
        private int count;

        public T this[int index]
        {
            get {
                var start = this;
                while(index > 8)
                {
                    index -= 8;
                    start = start.next;
                    if (start == null)
                        throw new IndexOutOfRangeException();
                }
                if (start.count <= index)
                    throw new IndexOutOfRangeException();
                return start.items[index];
            }
        }

        public string Description
        {
            get
            {
                var sb = new StringBuilder();
                var en = GetFastEnumerator();
                var isFirst = true;
                while(en.MoveNext(out var item))
                {
                    if (!isFirst)
                        sb.Append(',');
                    isFirst = false;
                    sb.Append(item);
                }
                return sb.ToString();
            }
        }

        public Sequence(bool allocate = false)
        {
            if (allocate)
                items = new T[8];
        }

        public void Add(T item)
        {
            if (items == null)
            {
                items = new T[8];
                items[count++] = item;
                return;
            }
            if (count < items.Length)
            {
                items[count++] = item;
                return;
            }
            next ??= new Sequence<T>();
            next.Add(item);
        }

        public int Count => count + (next?.Count ?? 0);

        public IEnumerator<T> GetEnumerator()
        {
            return GetFastEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetFastEnumerator();
        }

        public FastEnumerator<T> GetFastEnumerator()
        {
            return new FastSequenceEnumerator(this);
        }

        public class FastSequenceEnumerator : FastEnumerator<T>
        {
            private Sequence<T> start;
            private int index;

            public FastSequenceEnumerator(Sequence<T> start)
            {
                this.start = start;
                this.index = 0;
            }
            public override bool MoveNext(out T item)
            {
                if(start == null || start.count <= index)
                {
                    item = default;
                    return false;
                }
                item = start.items[index++];
                if (index == start.items.Length)
                {
                    start = start.next;
                    index = 0;
                }
                return true;
            }
        }
    }
    
    
}
