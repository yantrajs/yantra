using System.Collections.Generic;

namespace YantraJS.Core
{
    public interface IFastEnumerator<T>
    {
        bool MoveNext(out T item);

        bool MoveNext(out T item, out int index);

    }
    
    
}
