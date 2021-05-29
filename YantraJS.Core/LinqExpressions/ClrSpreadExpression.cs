using System;
using System.CodeDom.Compiler;
using System.Reflection;
using YantraJS.Core.Core.Array;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

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
        public ClrSpreadExpression(Expression argument): base(Expressions.YExpressionType.Constant, argument.Type)
        {
            this.Argument = JSSpreadValueBuilder.New( argument);
        }

        public Expression Argument { get; }

        public override void Print(IndentedTextWriter writer)
        {
            
        }
    }
}
