using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YantraJS
{
    public class LambdaRewriter: ExpressionVisitor
    {

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.VisitLambda(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }

        public static Expression Rewrite<T,TR>(Expression<Func<T, TR>> factory)
        {
            var l = new LambdaRewriter();
            return l.Visit(factory);
        }
    }
}
