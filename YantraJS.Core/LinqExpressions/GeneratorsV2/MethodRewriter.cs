using System.Collections.Generic;
using YantraJS.Expressions;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{
    public class YieldFinder: YExpressionMapVisitor
    {

        public static bool HasYield(YExpression exp)
        {
            var yf = new YieldFinder();
            yf.Visit(exp);
            return yf.hasYield;
        }

        private bool hasYield = false;

        public override Exp VisitIn(Exp exp)
        {
            if (hasYield)
                return exp;
            return base.VisitIn(exp);
        }

        protected override Exp VisitYield(YYieldExpression node)
        {
            hasYield = true;
            return node;
        }

        protected override Exp VisitReturn(YReturnExpression yReturnExpression)
        {
            hasYield = true;
            return yReturnExpression;
        }

        protected override Exp VisitLambda(YLambdaExpression yLambdaExpression)
        {
            return yLambdaExpression;
        }

        protected override Exp VisitRelay(YRelayExpression relayExpression)
        {
            return relayExpression;
        }
    }

    public class MethodRewriter
    {

        public static Expression Rewrite(YExpression exp)
        {
            var rw = new Rewriter();
            return rw.Visit(exp);

        }

        public class Rewriter: YExpressionMapVisitor
        {

            protected override Exp VisitLambda(YLambdaExpression yLambdaExpression)
            {
                return yLambdaExpression;
            }

            protected override Exp VisitRelay(YRelayExpression relayExpression)
            {
                return relayExpression;
            }

            protected override Expression VisitNew(YNewExpression node)
            {
                if (YieldFinder.HasYield(node))
                {
                    var argList = new List<ParameterExpression>();
                    var setup = new List<Expression>();
                    foreach (var a in node.args)
                    {
                        var p = Expression.Parameter(a.Type);
                        argList.Add(p);
                        setup.Add(Expression.Assign(p, Visit(a)));
                    }
                    setup.Add(Expression.New(node.constructor, argList));
                    return Expression.Block(argList.ToArray(),
                        setup);

                }
                return base.VisitNew(node);
            }

            protected override Expression VisitCall(YCallExpression node)
            {

                if(YieldFinder.HasYield(node))
                {
                    // rewrite...

                    var argList = new List<ParameterExpression>();
                    var setup = new List<Expression>();
                    foreach(var a in node.Arguments)
                    {
                        var p = Expression.Parameter(a.Type);
                        argList.Add(p);
                        setup.Add(Expression.Assign(p, Visit(a)));
                    }
                    setup.Add(Expression.Call(node.Target, node.Method, argList));
                    return Expression.Block(argList.ToArray(),
                        setup);
                }

                return node;
            }
        }

    }
}
