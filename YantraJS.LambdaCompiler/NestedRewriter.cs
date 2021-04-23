using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YantraJS
{
    public class NestedRewriter : ExpressionVisitor
    {
        private readonly LambdaExpression exp;
        private LambdaMethodBuilder lambdaMethodBuilder;

        public NestedRewriter(LambdaExpression exp, LambdaMethodBuilder lambdaMethodBuilder)
        {
            this.exp = exp;
            this.lambdaMethodBuilder = lambdaMethodBuilder;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (exp == node)
                return base.VisitLambda(node);

            return lambdaMethodBuilder.Create(node.Name, node);
        }
    }
}
