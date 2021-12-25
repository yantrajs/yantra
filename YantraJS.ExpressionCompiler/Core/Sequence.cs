using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core
{

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

        public Sequence(IEnumerable<T> items)
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
                Items = new T[tail.Count*2],
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
                Items = new T[tail.Count*2],
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
                Array.Copy(start.Items, 0, items, index, start.Count);
                index += start.Count;
                start = start.Next;
            }
            return items;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new FastSequenceEnumerator(head);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetFastEnumerator();
        }

        public FastSequenceEnumerator GetEnumerator()
        {
            return new FastSequenceEnumerator(head);
        }

        public FastSequenceEnumerator GetFastEnumerator()
        {
            return new FastSequenceEnumerator(head);
        }


        IFastEnumerator<T> IFastEnumerable<T>.GetFastEnumerator()
        {
            return new FastSequenceEnumerator(head);
        }

        public struct FastSequenceEnumerator : IFastEnumerator<T>
        {
            private Chain<T> start;
            private int position;
            private int current;
            private T currentItem;

            internal FastSequenceEnumerator(Chain<T> start)
            {
                this.start = start;
                this.position = 0;
                this.current = 0;
                currentItem = default;
            }

            public T Current => currentItem;

            object IEnumerator.Current => currentItem;

            public void Dispose()
            {
                
            }

            public bool MoveNext(out T item, out int index)
            {
                if (start == null || start.Count <= position)
                {
                    item = default;
                    index = default;
                    return false;
                }
                this.currentItem = start.Items[position++];
                item = this.currentItem;
                index = this.current++;
                if (position == start.Items.Length)
                {
                    start = start.Next;
                    position = 0;
                }
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext(out T item)
            {
                if(start == null || start.Count <= position)
                {
                    item = default;
                    return false;
                }
                this.currentItem = start.Items[position++];
                item = this.currentItem;
                if (position == start.Items.Length)
                {
                    start = start.Next;
                    position = 0;
                }
                return true;
            }

            public bool MoveNext()
            {
                return MoveNext(out var _);
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
    
    
}
