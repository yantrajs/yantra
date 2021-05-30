#nullable enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core;
using YantraJS.ExpHelper;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Expressions;

namespace YantraJS
{
    internal static class ExpressionHelper
    {
        public static void AddExpanded(
            this IList<Expression> list, 
            IList<ParameterExpression> peList,
            Expression exp)
        {

            if(exp.NodeType == YExpressionType.Block)
            {
                var block = (exp as YBlockExpression)!;
                foreach (var p in block.Variables)
                    peList.Add(p);
                foreach (var s in block.Expressions)
                    list.Add(s);
                return;
            }

            list.Add(exp);
        }


        public static Expression? ToJSValue(this Expression exp)
        {
            if (exp == null)
                return exp;
            if (typeof(JSVariable) == exp.Type)
                return JSVariable.ValueExpression(exp);
            if (typeof(JSValue) == exp.Type)
                return exp;
            if (!typeof(JSValue).IsAssignableFrom(exp.Type))
                return Expression.Block(exp, JSUndefinedBuilder.Value);
            // return Expression.Convert(exp, typeof(JSValue));
            return Expression.TypeAs(exp,typeof(JSValue));
        }

    }
}
