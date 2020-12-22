using System;
using System.Linq.Expressions;

namespace YantraJS.Core.LinqExpressions.Generators
{
    public class YieldExpression : Expression
    {
        public YieldExpression New(Expression argument)
        {
            return new YieldExpression(argument);
        }

        private YieldExpression(Expression argument)
        {
            Argument = argument;
        }

        public Expression Argument { get; }

        public override Type Type => Argument.Type;

        public override ExpressionType NodeType => ExpressionType.Extension;
    }
}
