using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public class JSArrayBuilder
    {
        private static Type type = typeof(JSArray);

        private static ConstructorInfo _New =
            type.GetConstructor(new Type[] { });

        private static MethodInfo _Add =
            type.GetMethod(nameof(Core.JSArray.Add), new Type[] { typeof(JSValue) });

        public static Expression New()
        {
            Expression start = Expression.New(_New);
            return start;
        }

        public static Expression Add(Expression target, Expression p)
        {
            return Expression.Call(target, _Add, p);
        }

        public static Expression New(IEnumerable<Expression> list)
        {
            Expression start = Expression.New(_New);
            foreach (var p in list)
            {
                start = Expression.Call(start, _Add, p);
            }
            return start;
        }

    }
}
