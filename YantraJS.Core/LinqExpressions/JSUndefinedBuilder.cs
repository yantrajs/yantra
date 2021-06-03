using System;
using System.Linq;
using System.Linq.Expressions;
using YantraJS.Core;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.ExpHelper
{
    public class JSUndefinedBuilder
    {
        public static Expression Value =
            Expression.Field(null,
                typeof(JSUndefined).GetField(nameof(Core.JSUndefined.Value)));
    }
}
