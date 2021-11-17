using System.Collections.Generic;

namespace YantraJS.Core
{
    public interface IFastEnumerable<T>: IEnumerable<T>
    {
        int Count { get; }

        T this[int index] { get; }

        IFastEnumerator<T> GetFastEnumerator();

        T First();

        T FirstOrDefault();

        T Last();

        T LastOrDefault();

        bool Any();

        T[] ToArray();
    }
    
    
}
