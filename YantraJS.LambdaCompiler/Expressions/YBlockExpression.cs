#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.Expressions
{
    public class YBlockExpression: YExpression
    {
        public readonly YParameterExpression[] Variables;
        public readonly YExpression[] Expressions;

        public YBlockExpression(IEnumerable<YParameterExpression>? variables, IList<YExpression> expressions)
            :base(YExpressionType.Block, expressions.Last().Type)
        {
            this.Variables = variables?.ToArray() ?? (new YParameterExpression[] { });
            this.Expressions = expressions.ToArray();
        }
    }
}