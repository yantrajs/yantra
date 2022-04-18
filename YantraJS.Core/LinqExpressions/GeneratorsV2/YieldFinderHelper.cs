using YantraJS.Expressions;
using Exp = YantraJS.Expressions.YExpression;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{
    public static class YieldFinderHelper
    {
        private static object yes = new object();
        private static object no = new object();

        private static System.Runtime.CompilerServices.ConditionalWeakTable<YExpression, object>
            cache = new System.Runtime.CompilerServices.ConditionalWeakTable<Exp, object>();

        public static bool HasYield(this YExpression expression)
        {
            if (cache.TryGetValue(expression, out var a))
            {
                return System.Object.ReferenceEquals(a, yes);
            }
            var r = YieldFinder.HasYield(expression);
            cache.Add(expression, r ? yes : no);
            return r;
        }


        public class YieldFinder : YExpressionMapVisitor
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

            //protected override Exp VisitRelay(YRelayExpression relayExpression)
            //{
            //    return relayExpression;
            //}
        }
    }
}
