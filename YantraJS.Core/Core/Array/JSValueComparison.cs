using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{
    internal class Comparer<T> : IComparer<T>
    {
        private readonly Comparison<T> cx;

        public static implicit operator Comparer<T>(Comparison<T> jv) => new Comparer<T>(jv);

        public Comparer(Comparison<T> cx)
        {
            this.cx = cx;
        }

        public int Compare(T x, T y)
        {
            return cx(x, y);
        }
    }
}
