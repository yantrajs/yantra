using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        T this[int index] { get; }

        FastEnumerator<T> GetFastEnumerator();

        T First();

        T FirstOrDefault();

        T Last();

        T LastOrDefault();
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

    public class EnumerableSequence<T> : IFastEnumerable<T>
    {
        private readonly IEnumerable<T> enumerable;

        public EnumerableSequence(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
        }

        public T this[int index] => enumerable.ElementAt(index);

        public int Count => enumerable.Count();

        public T First()
        {
            return enumerable.First();
        }

        public T FirstOrDefault()
        {
            return enumerable.FirstOrDefault();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return enumerable.GetEnumerator();
        }

        public FastEnumerator<T> GetFastEnumerator()
        {
            return new EnumerableEnumerator(enumerable.GetEnumerator());
        }

        public T Last()
        {
            return enumerable.Last();
        }

        public T LastOrDefault()
        {
            return enumerable.LastOrDefault();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class EnumerableEnumerator : FastEnumerator<T>
        {
            private readonly IEnumerator<T> en;
            private int index;

            public EnumerableEnumerator(IEnumerator<T> en)
            {
                this.en = en;
                this.index = 0;
            }

            public override bool MoveNext(out T item)
            {
                if (en.MoveNext())
                {
                    item = en.Current;
                    return true;
                }
                item = default;
                return false;
            }

            public override bool MoveNext(out T item, out int index)
            {
                if (en.MoveNext())
                {
                    item = en.Current;
                    index = this.index++;
                    return true;
                }
                item = default;
                index = default;
                return false;

            }
        }
    }

    public static class Sequence
    {

        public static IFastEnumerable<T> AsSequence<T>(this IEnumerable<T> items)
        {
            return new EnumerableSequence<T>(items);
        }

        public static Sequence<T> AsSequence<T>(this T[] items)
        {
            return new Sequence<T>(items);
        }

        public static SingleElementSequence<T> AsSequence<T>(this T item) => new SingleElementSequence<T>(item);
    }

    public class SingleElementSequence<T> : IFastEnumerable<T>
    {
        private readonly T item;

        public SingleElementSequence(T item)
        {
            this.item = item;
        }

        public T this[int index] => index == 0 ? item : throw new IndexOutOfRangeException();

        public int Count => 1;

        public T First()
        {
            return item;
        }

        public T FirstOrDefault()
        {
            return item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetFastEnumerator();
        }

        public FastEnumerator<T> GetFastEnumerator()
        {
            return new SingleSequenceEnumerator(item);
        }

        public T Last()
        {
            return item;
        }

        public T LastOrDefault()
        {
            return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetFastEnumerator();
        }

        private class SingleSequenceEnumerator : FastEnumerator<T>
        {
            private readonly T item;
            private bool done;

            public SingleSequenceEnumerator(T item)
            {
                this.item = item;
            }

            public override bool MoveNext(out T item)
            {
                if (done)
                {
                    item = default;
                    return false;
                }
                done = true;
                item = this.item;
                return true;
            }

            public override bool MoveNext(out T item, out int index)
            {
                index = 0;
                if (done)
                {
                    item = default;
                    return false;
                }
                done = true;
                item = this.item;
                return true;
            }
        }
    }

    public class Sequence<T>: IReadOnlyList<T>, IFastEnumerable<T>
    {

        public static Sequence<T> Empty = new Sequence<T>();

        // public static implicit operator Sequence<T>(T[] items) => new Sequence<T>(items);


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

        public Sequence(T[] items)
        {
            if (items.Length > 0)
            {
                var t = new Chain<T>
                {
                    Items = items,
                    Count = items.Length
                };
                this.count = items.Length;
                this.head = t;
                this.tail = t;
            }
        }

        internal Sequence(IEnumerable<T> items)
        {
            AddRange(items);
        }

        public Sequence(IFastEnumerable<T> items)
        {
            var all = items.ToArray();
            if (all.Length > 0)
            {
                var t = new Chain<T>
                {
                    Items = all,
                    Count = all.Length
                };
                this.count = all.Length;
                this.head = t;
                this.tail = t;
            }
        }

        public Sequence(int capacity)
        {
            if (capacity > 0)
            {
                this.head = new Chain<T>
                {
                    Items = new T[capacity]
                };
                this.tail = this.head;
            }
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

        public ref T AddGetRef()
        {
            if (tail == null)
            {
                var h = new Chain<T>
                {
                    Items = new T[8],
                    Count = 1
                };
                // h.Items[0] = item;
                head = h;
                tail = h;
                count++;
                return ref h.Items[0];
            }
            if (tail.Count < tail.Items.Length)
            {
                count++;
                return ref tail.Items[tail.Count++];
                // return;
            }
            var t = new Chain<T>
            {
                Items = new T[8],
                Count = 1
            };
            // t.Items[0] = item;
            tail.Next = t;
            tail = t;
            count++;
            return ref t.Items[0];
        }

        public void Insert(int i, T item)
        {
            Add(default);
            for (int index = count - 2 ; index >= i; index--)
            {
                this[index + 1] = this[index];
            }
            this[i] = item;
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

        public bool Any() => count > 0;


        public int Count => count;

        public T First()
        {
            if(count > 0)
            {
                return head.Items[0];
            }
            throw new IndexOutOfRangeException();
        }

        public T FirstOrDefault()
        {
            if (count > 0)
            {
                return head.Items[0];
            }
            return default;
        }

        public T Last()
        {
            if (tail != null && tail.Count>0)
            {
                return tail.Items[tail.Count - 1];
            }
            throw new IndexOutOfRangeException();
        }

        public T LastOrDefault()
        {
            if (tail != null && tail.Count > 0)
            {
                return tail.Items[tail.Count - 1];
            }
            return default;
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
            private int current;

            internal FastSequenceEnumerator(Chain<T> start)
            {
                this.start = start;
                this.position = 0;
                this.current = 0;
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
                index = this.current++;
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
