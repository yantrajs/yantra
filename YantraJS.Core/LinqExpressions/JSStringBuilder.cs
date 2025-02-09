using System;
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
using YantraJS.Core.LambdaGen;

namespace YantraJS.ExpHelper
{

    public class JSStringBuilder 
    {


        //private static FieldInfo _Value =
        //    typeof(JSString).InternalField(nameof(Core.JSString.Value));

        //public static Expression Value(Expression ex)
        //{
        //    return Expression.Field(ex, _Value);
        //}

        // private static ConstructorInfo _New = typeof(JSString).Constructor(typeof(string));

        public static Expression New(Expression exp)
        {
            return
                Expression.TypeAs(
                    NewLambdaExpression.NewExpression<JSString>(() => () => new JSString(""), exp)
                    , typeof(JSValue));
            // return Expression.TypeAs( Expression.New(_New, exp), typeof(JSValue));
        }

        //public static Expression ConcatBasicStrings(Expression left, Expression right)
        //{
        //    return Expression.New(_New, ClrStringBuilder.Concat(left, right));
        //}

    }
}
