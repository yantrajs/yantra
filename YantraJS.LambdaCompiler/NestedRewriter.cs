using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS
{
    public class NestedRewriter : YExpressionMapVisitor
    {
        private readonly YLambdaExpression exp;
        private IMethodBuilder lambdaMethodBuilder;

        public NestedRewriter(YLambdaExpression exp, IMethodBuilder lambdaMethodBuilder)
        {
            this.exp = exp;
            this.lambdaMethodBuilder = lambdaMethodBuilder;
        }

        protected override YExpression VisitRelay(YRelayExpression relayExpression)
        {

            var inner = Visit(relayExpression.InnerLambda);

            return lambdaMethodBuilder.Relay(relayExpression.Closures, inner as YLambdaExpression);
            // return base.VisitRelay(relayExpression);
        }

        //protected override YExpression VisitLambda(YLambdaExpression node)
        //{
        //    if (exp == node)
        //        return base.VisitLambda(node);

        //    return lambdaMethodBuilder.Create(node.Name, node);
        //}
    }
}
