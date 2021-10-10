using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core
{
    public static class FastEnumerableExtensions
    {
        public static T[] ToArray<T>(this IFastEnumerable<T> enumerable)
        {
            var total = enumerable.Count;
            var en = enumerable.GetFastEnumerator();
            var items = new T[total];
            int index = 0;
            while(en.MoveNext(out var item))
            {
                items[index++] = item;
            }
            return items;
        }
        public static string Join<T>(this IFastEnumerable<T> enumerable, string separator = ", ")
        {
            var sb = new StringBuilder();
            var en = enumerable.GetFastEnumerator();
            bool first = true;
            while (en.MoveNext(out var item))
            {
                if (!first)
                {
                    sb.Append(separator);
                }
                first = false;
                sb.Append(item);
            }
            return sb.ToString();
        }

    }

    public  interface IFastEnumerable<T>: IEnumerable<T>
    {
        int Count { get; }

        FastEnumerator<T> GetFastEnumerator();

        T First();

        T Last();
    }

    public abstract class FastEnumerator<T>: IEnumerator<T>
    {
        public T Current { get; set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            
        }

        public abstract bool MoveNext(out T item);

        public virtual bool MoveNext(out T item, out int index) => throw new NotImplementedException();

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

    internal class Chain<T>
    {
        public T[] Items;
        public int Count;
        public Chain<T> Next;
    }

    public class Sequence<T>: IReadOnlyList<T>, IFastEnumerable<T>
    {

        public static Sequence<T> Empty = new Sequence<T>();


        private Chain<T> head;
        private Chain<T> tail;
        private int count;

        public T this[int index]
        {
            get {
                if (index >= count)
                    throw new IndexOutOfRangeException();
                var start = head;
                while(index >= start.Count)
                {
                    index -= start.Count;
                    start = start.Next;
                }
                return start.Items[index];
            } set {
                if (index >= count)
                    throw new IndexOutOfRangeException();
                var start = head;
                while (index >= start.Count)
                {
                    index -= start.Count;
                    start = start.Next;
                }
                start.Items[index] = value;
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

        public Sequence()
        {
        }

        public Sequence(int capacity)
        {
            this.head = new Chain<T> { 
                Items = new T[capacity]
            };
            this.tail = this.head;
        }

        public string Join(string separator = ", ")
        {
            var sb = new StringBuilder();
            var en = new FastSequenceEnumerator(head);
            bool first = true;
            while(en.MoveNext(out var item))
            {
                if (!first)
                {
                    sb.Append(separator);
                }
                first = false;
                sb.Append(item);
            }
            return sb.ToString();
        }

        public void Add(T item)
        {
            if(tail == null)
            {
                var h = new Chain<T> { 
                    Items = new T[8],
                    Count = 1
                };
                h.Items[0] = item;
                head = h;
                tail = h;
                count++;
                return;
            }
            if (tail.Count < tail.Items.Length)
            {
                tail.Items[tail.Count++] = item;
                count++;
                return;
            }
            var t = new Chain<T> { 
                Items = new T[8],
                Count = 1
            };
            t.Items[0] = item;
            tail.Next = t;
            tail = t;
            count++;
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach(var item in range)
            {
                Add(item);
            }
        }

        public void AddRange(Sequence<T> range)
        {
            var en = range.GetFastEnumerator();
            while (en.MoveNext(out var item))
                Add(item);
        }


        public int Count => count;

        public T First()
        {
            if(count > 0)
            {
                return head.Items[0];
            }
            throw new IndexOutOfRangeException();
        }

        public T Last()
        {
            if (tail != null && tail.Count>0)
            {
                return tail.Items[tail.Count - 1];
            }
            throw new IndexOutOfRangeException();
        }

        public T FirstOrDefault(Func<T,bool> predicate)
        {
            var e = new FastSequenceEnumerator(head);
            while(e.MoveNext(out var item))
            {
                if (predicate(item))
                    return item;
            }
            return default;
        }

        public T FirstOrDefault<T1>(T1 param, Func<T, T1, bool> predicate)
        {
            var e = new FastSequenceEnumerator(head);
            while (e.MoveNext(out var item))
            {
                if (predicate(item, param))
                    return item;
            }
            return default;
        }

        public T[] ToArray()
        {
            var items = new T[Count];
            var start = this.head;
            int index = 0;
            while (start != null)
            {
                for (int i = 0; i < start.Count; i++)
                {
                    items[index++] = start.Items[i];
                }
                start = start.Next;
            }
            return items;
        }

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
            return new FastSequenceEnumerator(head);
        }

        public class FastSequenceEnumerator : FastEnumerator<T>
        {
            private Chain<T> start;
            private int position;
            private int index;

            internal FastSequenceEnumerator(Chain<T> start)
            {
                this.start = start;
                this.position = 0;
                this.index = 0;
            }

            public override bool MoveNext(out T item, out int index)
            {
                if (start == null || start.Count <= position)
                {
                    item = default;
                    index = default;
                    return false;
                }
                item = start.Items[position++];
                index = this.index++;
                if (position == start.Items.Length)
                {
                    start = start.Next;
                    position = 0;
                }
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool MoveNext(out T item)
            {
                if(start == null || start.Count <= position)
                {
                    item = default;
                    return false;
                }
                item = start.Items[position++];
                if (position == start.Items.Length)
                {
                    start = start.Next;
                    position = 0;
                }
                return true;
            }
        }
    }
    
    
}
