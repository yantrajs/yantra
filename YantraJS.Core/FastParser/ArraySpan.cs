﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public static class ArraySpanHelper
    {
        public static ArraySpan<T> ToArraySpan<T>(this T[] items)
        {
            return new ArraySpan<T>(items, items.Length);
        }

        public static ArraySpan<T> ToArraySpan<T>(this T[] items, int length)
        {
            return new ArraySpan<T>(items, length);
        }

    }

    public readonly struct ArraySpan<T>: IEnumerable<T>
    {

        private readonly T[] items;
        public readonly int Length;

        public static ArraySpan<T> Empty;

        public ArraySpan(T[] items, int length)
        {
            this.items = items;
            this.Length = length;
        }

        public ref T this[int index] => ref this.items[index];

        public static implicit operator ArraySpan<T>(FastList<T> source)
        {
            return source?.ToSpan() ?? Empty;
        }

        public static ArraySpan<T> From(params T[] items)
        {
            return new ArraySpan<T>(items, items.Length);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(items, Length);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(items, Length);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(items, Length);
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly T[] items;
            private readonly int length;
            private int index;

            public Enumerator(T[] items, int length)
            {
                this.items = items;
                this.length = length;
                index = -1;
            }

            public bool MoveNext(out T item)
            {
                if(++index < length )
                {
                    item = items[index];
                    return true;
                }
                item = default;
                return false;
            }

            public T Current => items[index];

            object IEnumerator.Current => items[index];

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                return ++index < length;
            }

            public void Reset()
            {
                index = -1;
            }
        }

        internal void Copy(T[] copy, int start)
        {
            Array.Copy(items, 0, copy, start, Length);
        }
    }
}