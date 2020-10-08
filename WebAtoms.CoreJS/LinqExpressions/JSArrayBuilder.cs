using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSArrayBuilder : TypeHelper<Core.JSArray>
    {
        private static ConstructorInfo _New =
            typeof(Core.JSArray).GetConstructor(new Type[] { });

        private static MethodInfo _Add =
            Method<Core.JSValue>(nameof(Core.JSArray.Add));
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
