using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YantraJS
{
    public class BaseTest
    {

        public T Simple<T>(Func<T> x) {
            return x();
        }

        public Expression<Func<T1, T>> Get<T1, T>(Expression<Func<T1, T>> exp) => exp;
        public Expression<Func<T1, T2, T>> Get<T1, T2, T>(Expression<Func<T1, T2, T>> exp) => exp;

        public Expression<Func<T1, T2, T3, T>> Get<T1, T2, T3, T>(Expression<Func<T1, T2, T3, T>> exp) => exp;
    }
}
