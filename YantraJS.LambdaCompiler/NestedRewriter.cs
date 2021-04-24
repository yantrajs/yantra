using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS
{
    public class NestedRewriter : YExpressionMapVisitor
    {
        private readonly YLambdaExpression exp;
        private LambdaMethodBuilder lambdaMethodBuilder;

        public NestedRewriter(YLambdaExpression exp, LambdaMethodBuilder lambdaMethodBuilder)
        {
            this.exp = exp;
            this.lambdaMethodBuilder = lambdaMethodBuilder;
        }

        protected override YExpression VisitLambda(YLambdaExpression node)
        {
            if (exp == node)
                return base.VisitLambda(node);

            return lambdaMethodBuilder.Create(node.Name, node);
        }
    }
}
