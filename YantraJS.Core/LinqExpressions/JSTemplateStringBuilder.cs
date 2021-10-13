using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core.String;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Expressions;
using YantraJS.Core;

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

        private static MethodInfo _addString =
            type.PublicMethod(nameof(JSTemplateString.Add), typeof(string));

        private static MethodInfo _addValue =
            type.PublicMethod(nameof(JSTemplateString.Add), typeof(JSValue));

        public static Expression New(IEnumerable<Expression> select, int total)
        {
            var list = new Sequence<YElementInit>();
            var newExp = Expression.New(_new, Expression.Constant(total));
            var en = select.GetEnumerator();
            while (en.MoveNext())
            {
                var current = en.Current;
                if (current.NodeType == YExpressionType.Constant)
                {
                    // exp = Expression.Call(exp, _addQuasi, current);
                    list.Add(YExpression.ElementInit(_addString, current));
                    continue;
                }
                list.Add(YExpression.ElementInit(_addValue, current));
            }
            return Expression.Call( Expression.ListInit(newExp, list), _toJSString);
        }

        //public static Expression New(List<string> quasis, IEnumerable<Expression> select)
        //{
        //    var total = quasis.Sum(x => x.Length);
        //    Expression exp = Expression.New(_new, Expression.Constant(total));
        //    var qn = quasis.GetEnumerator();
        //    var en = select.GetEnumerator();
        //    bool end = false;
        //    while (!end)
        //    {
        //        end = true;
        //        if (qn.MoveNext())
        //        {
        //            var ec = qn.Current;
        //            if (ec.Length > 0)
        //            {
        //                exp = Expression.Call(exp, _addQuasi, Expression.Constant(ec));
        //            }
        //            end = false;
        //        }
        //        if (en.MoveNext())
        //        {
        //            var ec = en.Current;
        //            exp = Expression.Call(exp, _addExpression, ec);
        //        }
        //    }
        //    return Expression.Call(exp, _toJSString);
        //}

    }
}
