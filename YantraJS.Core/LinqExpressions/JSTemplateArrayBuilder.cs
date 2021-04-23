using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YantraJS.Core.Extensions;

namespace YantraJS.Core.LinqExpressions
{
    public static class JSTemplateArrayBuilder
    {

        public static Type type = typeof(JSTemplateArray);

        private static MethodInfo _new =
            type.PublicMethod(nameof(JSTemplateArray.New), typeof(string[]), typeof(string[]), typeof(JSValue[]));

        public static Expression New(IList<Expression> cooked, IList<Expression> raw, IList<Expression> args)
        {

            var c = Expression.NewArrayInit(typeof(string), cooked);
            var r = Expression.NewArrayInit(typeof(string), raw);
            var v = Expression.NewArrayInit(typeof(JSValue), args);
            return Expression.Call(null, _new, c, r, v);
        }

    }
}
