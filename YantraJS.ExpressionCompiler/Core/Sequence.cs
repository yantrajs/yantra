using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core
{

    public class Sequence<T>: IReadOnlyList<T>, IFastEnumerable<T>
    {

        public const int DefaultCapacity = 4;

        public static IFastEnumerable<T> Empty = new Sequence<T>();

        // public static implicit operator Sequence<T>(T[] items) => new Sequence<T>(items);


        class Node
        {
            internal T[] Items;
            internal Node Next;
        }
        private int count;
        private Node head;
        private Node tail;
        private T[] tailArray = Array.Empty<T>();
        private int tailCount;

        public T this[int index]
        {
            get {
                if (index >= count)
                    throw new IndexOutOfRangeException();
                var start = head;
                while(start != tail)
                {
                    var len = start.Items.Length;
                    if (index < len)
                    {
                        return start.Items[index];
                    }
                    index -= len;
                    start = start.Next;
                }
                return start.Items[index];
            } set {
                if (index >= count)
                    throw new IndexOutOfRangeException();
                var start = head;
                while (start != tail)
                {
                    var len = start.Items.Length;
                    if (index < len)
                    {
                        start.Items[index] = value;
                        return;
                    }
                    index -= len;
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
                tailArray = items;
                tailCount = items.Length;
                var t = new Node
                {
                    Items = items,
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
                tailArray = all;
                tailCount = all.Length;
                var t = new Node
                {
                    Items = all,
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
                tailArray = new T[capacity];
                this.head = new Node
                {
                    Items = tailArray
                };
                this.tail = this.head;
                
            }
        }

        public string Join(string separator = ", ")
        {
            var sb = new StringBuilder();
            var en = new FastSequenceEnumerator(this);
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
            if (tailCount < tailArray.Length)
            {
                count++;
                return ref tailArray[tailCount++];
            }

            if (head == null)
            {
                tailArray = new T[DefaultCapacity];
                tailCount = 1;
                var t = new Node
                {
                    Items = tailArray,
                };
                head = t;
                tail = t;
            }
            else
            {
                tailArray = new T[count];
                tailCount = 1;
                var t = new Node
                {
                    Items = tailArray,
                };
                tail.Next = t;
                tail = t;
            }
            count++;
            return ref tailArray[0];
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
            if (tailCount < tailArray.Length)
            {
                tailArray[tailCount++] = item;
                count++;
                return;
            }

            if (head == null)
            {
                tailArray = new T[DefaultCapacity];
                tailArray[0] = item;
                tailCount = 1;
                var t = new Node
                {
                    Items = tailArray,
                };
                head = t;
                tail = t;
            }
            else
            {
                tailArray = new T[count];
                tailArray[0] = item;
                tailCount = 1;
                var t = new Node
                {
                    Items = tailArray,
                };
                tail.Next = t;
                tail = t;
            }
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
            if (tailArray != null && tailCount > 0)
            {
                return tailArray[tailCount - 1];
            }
            throw new IndexOutOfRangeException();
        }

        public T LastOrDefault()
        {
            if (tailArray != null && tailCount > 0)
            {
                return tailArray[tailCount - 1];
            }
            return default;
        }

        public T FirstOrDefault(Func<T,bool> predicate)
        {
            var e = new FastSequenceEnumerator(this);
            while(e.MoveNext(out var item))
            {
                if (predicate(item))
                    return item;
            }
            return default;
        }

        public T FirstOrDefault<T1>(T1 param, Func<T, T1, bool> predicate)
        {
            var e = new FastSequenceEnumerator(this);
            while (e.MoveNext(out var item))
            {
                if (predicate(item, param))
                    return item;
            }
            return default;
        }

        public T[] ToArray()
        {
            if (count == 0)
            {
                return Array.Empty<T>();
            }
            var items = new T[count];
            var start = this.head;
            var last = this.tail;
            int index = 0;
            while (start != last)
            {
                Array.Copy(start.Items, 0, items, index, start.Items.Length);
                index += start.Items.Length;
                start = start.Next;
            }
            Array.Copy(tailArray, 0, items, index, tailCount);
            return items;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new FastSequenceEnumerator(this);
        }

        //IEnumerator<T> IFastEnumerable<T>.GetEnumerator()
        //{
        //    return new FastSequenceEnumerator(this);
        //}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetFastEnumerator();
        }

        public FastSequenceEnumerator GetEnumerator()
        {
            return new FastSequenceEnumerator(this);
        }

        public FastSequenceEnumerator GetFastEnumerator()
        {
            return new FastSequenceEnumerator(this);
        }


        IFastEnumerator<T> IFastEnumerable<T>.GetFastEnumerator()
        {
            return new FastSequenceEnumerator(this);
        }

        public struct FastSequenceEnumerator : IFastEnumerator<T>, IEnumerator<T>
        {
            private Node start;
            private readonly int max;
            private int position;
            private int current;
            private T currentItem;

            internal FastSequenceEnumerator(Sequence<T> s)
            {
                this.start = s.head;
                this.max = s.count;
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
                if (current >= max)
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
                if(current >= max)
                {
                    item = default;
                    return false;
                }
                this.currentItem = start.Items[position++];
                current++;
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
