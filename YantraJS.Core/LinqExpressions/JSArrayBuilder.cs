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
using YantraJS.Expressions;

namespace YantraJS.ExpHelper
{
    public class JSArrayBuilder
    {
        private static Type type = typeof(JSArray);

        public static ConstructorInfo _New =
            type.GetConstructor(new Type[] { });

        private static ConstructorInfo _NewFromElementEnumerator =
            type.GetConstructor(new Type[] { typeof(IElementEnumerator) });

        public static MethodInfo _Add =
            type.GetMethod(nameof(Core.JSArray.Add), new Type[] { typeof(JSValue) });

        public static MethodInfo _AddRange =
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

        public static Expression New(IFastEnumerable<YElementInit> inits)
        {
            return Expression.ListInit(Expression.New(_New), inits);
        }

        public static Expression New(IFastEnumerable<Expression> list)
        {
            var ei = new Sequence<YElementInit>(list.Count());
            var en = list.GetFastEnumerator();
            while(en.MoveNext(out var e))
            {
                ei.Add(Expression.ElementInit(_Add, new YExpression[] { e }));
            }
            return Expression.ListInit(Expression.New(_New), ei);
            //Expression start = Expression.New(_New);
            //foreach (var p in list)
            //{
            //    start = Expression.Call(start, _Add, p);
            //}
            //return start;
        }

        public static Expression NewFromElementEnumerator(Expression en)
        {
            return Expression.New(_NewFromElementEnumerator, en);
        }


    }
}
