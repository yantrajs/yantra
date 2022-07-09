using System;
using System.Collections;
using System.Collections.Generic;

namespace YantraJS.Core
{
    public abstract class FastEnumerator<T>: IFastEnumerator<T>, IEnumerator<T>
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
    
    
}
