using System.Collections.Generic;

namespace YantraJS.Core
{
    public static class SequenceExtensions
    {

        public static IFastEnumerable<T> AsSequence<T>(this IEnumerable<T> items)
        {
            return new EnumerableSequence<T>(items);
        }

        public static IFastEnumerable<T> AsSequence<T>(this List<T> items)
        {
            return new EnumerableSequence<T>(items);
        }

        public static Sequence<T> AsSequence<T>(this T[] items)
        {
            return new Sequence<T>(items);
        }

        public static SingleElementSequence<T> AsSequence<T>(this T item) => new SingleElementSequence<T>(item);
    }
    
    
}
