using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YantraJS.Core.Core.Array;

namespace YantraJS.Core.LinqExpressions
{

    internal class JSSpreadValueBuilder
    {
        internal static Type type = typeof(JSSpreadValue);

        internal static ConstructorInfo _new
            = type.Constructor(typeof(JSValue));

        public static Expression New(Expression target)
        {
            return Expression.New(_new, target);
        }
    }

    public class ClrSpreadExpression : Expression
    {
        public ClrSpreadExpression(Expression argument)
        {
            this.Argument = JSSpreadValueBuilder.New( argument);
        }

        public Expression Argument { get; }
    }
}
