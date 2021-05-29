#nullable enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core;
using YantraJS.ExpHelper;

namespace YantraJS
{
    internal static class ExpressionHelper
    {
        public static void AddExpanded(
            this IList<Expression> list, 
            IList<ParameterExpression> peList,
            Expression exp)
        {

            if(exp.NodeType == ExpressionType.Block)
            {
                var block = (exp as BlockExpression)!;
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
