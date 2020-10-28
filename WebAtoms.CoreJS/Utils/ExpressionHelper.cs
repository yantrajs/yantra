using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.ExpHelper;

namespace WebAtoms.CoreJS
{
    internal static class ExpressionHelper
    {

        public static Expression ToJSValue(this Expression exp)
        {
            if (exp == null)
                return exp;
            if (typeof(JSVariable) == exp.Type)
                return JSVariable.ValueExpression(exp);
            if (typeof(JSValue) != exp.Type)
                return Expression.Block(exp, JSUndefinedBuilder.Value);
            return Expression.TypeAs(exp,typeof(JSValue));
        }

    }
}
