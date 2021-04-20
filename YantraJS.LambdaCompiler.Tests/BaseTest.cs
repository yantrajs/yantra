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

    }
}
