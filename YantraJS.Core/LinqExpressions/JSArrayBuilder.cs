using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;

namespace YantraJS.ExpHelper
{
    public class JSArrayBuilder
    {
        private static Type type = typeof(JSArray);

        private static ConstructorInfo _New =
            type.GetConstructor(new Type[] { });

        private static ConstructorInfo _NewFromElementEnumerator =
            type.GetConstructor(new Type[] { typeof(IElementEnumerator) });

        private static MethodInfo _Add =
            type.GetMethod(nameof(Core.JSArray.Add), new Type[] { typeof(JSValue) });

        private static MethodInfo _AddRange =
            type.GetMethod(nameof(Core.JSArray.AddRange), new Type[] { typeof(JSValue) });


        public static Expression New()
        {
            Expression start = Expression.New(_New);
            return start;
        }

        public static Expression Add(Expression target, Expression p)
        {
            return Expression.Call(target, _Add, p);
        }

        public static Expression AddRange(Expression target, Expression p)
        {
            return Expression.Call(target, _AddRange, p);
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

        public static Expression NewFromElementEnumerator(Expression en)
        {
            return Expression.New(_NewFromElementEnumerator, en);
        }


    }
}
