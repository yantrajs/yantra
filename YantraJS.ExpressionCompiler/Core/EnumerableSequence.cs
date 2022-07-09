using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.Core
{
    public struct EnumerableSequence<T> : IFastEnumerable<T>
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

        public EnumerableEnumerator GetFastEnumerator()
        {
            return new EnumerableEnumerator(enumerable.GetEnumerator());
        }

        IFastEnumerator<T> IFastEnumerable<T>.GetFastEnumerator()
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

        public bool Any()
        {
            return enumerable.Any();
        }

        public T[] ToArray()
        {
            return enumerable.ToArray();
        }

        public struct EnumerableEnumerator : IFastEnumerator<T>, IEnumerator<T>
        {
            private readonly IEnumerator<T> en;
            private int index;

            public EnumerableEnumerator(IEnumerator<T> en)
            {
                this.en = en;
                this.index = 0;
            }

            public T Current => en.Current;

            object IEnumerator.Current => en.Current;

            public void Dispose()
            {
                
            }

            public bool MoveNext(out T item)
            {
                if (en.MoveNext())
                {
                    item = en.Current;
                    return true;
                }
                item = default;
                return false;
            }

            public bool MoveNext(out T item, out int index)
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

            public bool MoveNext()
            {
                return en.MoveNext();
            }

            public void Reset()
            {
                
            }
        }
    }
    
    
}
