using System;
using System.Collections;
using System.Collections.Generic;

namespace YantraJS.Core
{
    public struct SingleElementSequence<T> : IFastEnumerable<T>
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

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetFastEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SingleSequenceEnumerator(item);
        }

        public SingleSequenceEnumerator GetFastEnumerator()
        {
            return new SingleSequenceEnumerator(item);
        }

        IFastEnumerator<T> IFastEnumerable<T>.GetFastEnumerator()
        {
            return GetFastEnumerator();
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

        public bool Any()
        {
            return true;
        }

        public T[] ToArray()
        {
            return new T[] { item };
        }

        public struct SingleSequenceEnumerator : IFastEnumerator<T>, IEnumerator<T>
        {
            private readonly T item;
            private bool done;

            public SingleSequenceEnumerator(T item)
            {
                this.item = item;
                done = false;
            }

            public T Current => item;

            object IEnumerator.Current => item;

            public void Dispose()
            {
                
            }

            public bool MoveNext(out T item)
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

            public bool MoveNext(out T item, out int index)
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

            public bool MoveNext()
            {
                if (done)
                {
                    return false;
                }
                done = true;
                return true;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
    
    
}
