using System.Text;

namespace YantraJS.Core
{
    public static class FastEnumerableExtensions
    {
        public static T[] ToArray<T>(this IFastEnumerable<T> enumerable)
        {
            if (enumerable is Sequence<T> sequence)
                return sequence.ToArray();
            var total = enumerable.Count;
            var en = enumerable.GetFastEnumerator();
            var items = new T[total];
            int index = 0;
            while(en.MoveNext(out var item))
            {
                items[index++] = item;
            }
            return items;
        }
        public static string Join<T>(this IFastEnumerable<T> enumerable, string separator = ", ")
        {
            var sb = new StringBuilder();
            var en = enumerable.GetFastEnumerator();
            bool first = true;
            while (en.MoveNext(out var item))
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

    }
    
    
}
