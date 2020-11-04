using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core.String;

namespace YantraJS.ExpHelper
{
    public class JSTemplateStringBuilder
    {
        private static Type type = typeof(JSTemplateString);

        private static ConstructorInfo _new =
            type.GetConstructor(new Type[] { typeof(int) });

        private static MethodInfo _addQuasi =
            type.GetMethod(nameof(JSTemplateString.AddQuasi));

        private static MethodInfo _addExpression =
            type.GetMethod(nameof(JSTemplateString.AddExpression));

        private static MethodInfo _toJSString =
            type.GetMethod(nameof(JSTemplateString.ToJSString));

        public static Expression New(List<string> quasis, IEnumerable<Expression> select)
        {
            var total = quasis.Sum(x => x.Length);
            Expression exp = Expression.New(_new, Expression.Constant(total));
            var qn = quasis.GetEnumerator();
            var en = select.GetEnumerator();
            bool end = false;
            while (!end)
            {
                end = true;
                if (qn.MoveNext())
                {
                    var ec = qn.Current;
                    if (ec.Length > 0)
                    {
                        exp = Expression.Call(exp, _addQuasi, Expression.Constant(ec));
                    }
                    end = false;
                }
                if (en.MoveNext())
                {
                    var ec = en.Current;
                    exp = Expression.Call(exp, _addExpression, ec);
                }
            }
            return Expression.Call(exp, _toJSString);
        }

    }
}
