using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Core.Storage
{
    public class FastLinkedList<T>
        where T : FastLinkedList<T>.FastLinkedListNode
    {

        public T First { get; private set; }
        public T Last { get; private set; }

        public bool IsEmpty => First == null;

        public void AddFirst(T item)
        {
            if (item.Previous != null || item.Next != null)
                throw new ArgumentException("Item already exists in the list");
            var first = First;
            if (first == null)
            {
                First = item;
                Last = item;
                return;
            }
            if (Last.Previous == null)
            {
                Last.Next = item;
                item.Next = Last;
            }
            item.Next = First;
            First.Previous = item;
            First = item;
        }

        public T RemoveFirst()
        {
            if (First == null)
                return null;
            var first = First;
            First = first.Next;
            First.Previous = null;
            first.Previous = null;
            first.Next = null;
            if (Last == first)
                Last = null;
            return first;
        }

        public T RemoveLast()
        {
            if (Last == null)
                return null;
            var last = Last;
            Last = last.Previous;
            Last.Next = null;
            last.Next = null;
            last.Previous = null;
            if (First == last)
                First = null;
            return last;
        }

        public T Next(T item) => item.Next;

        public T Previous(T item) => item.Previous;

        public void AddLast(T item)
        {
            if (item.Previous != null || item.Next != null)
                throw new ArgumentException("Item already exists in the list");
            var last = Last;
            if (last == null)
            {
                Last = item;
                First = item;
                return;
            }
            if (First.Next == null)
            {
                First.Next = item;
                item.Previous = First;
            }
            item.Previous = last;
            last.Next = item;
            Last = item;

        }

        public string CSV()
        {
            StringBuilder sb = new StringBuilder();
            var f = First;
            while (f != null)
            {
                if (f != First)
                {
                    sb.Append(',');
                }
                sb.Append(f.ToString());
                f = f.Next;
            }
            return sb.ToString();
        }

        public abstract class FastLinkedListNode
        {
            internal T Next;
            internal T Previous;
        }

    }
}
