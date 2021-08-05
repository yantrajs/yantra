using System.Collections.Generic;
using YantraJS.Expressions;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{
    public static class YieldFinderHelper
    {
        private static object empty = new object();

        private static System.Runtime.CompilerServices.ConditionalWeakTable<YExpression, object>
            cache = new System.Runtime.CompilerServices.ConditionalWeakTable<Exp, object>();

        public static bool HasYield(this YExpression expression)
        {
            if (cache.TryGetValue(expression, out var a))
                return a != empty;
            var r = YieldFinder.HasYield(expression);
            cache.Add(expression, r ? empty : null);
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

            protected override Exp VisitRelay(YRelayExpression relayExpression)
            {
                return relayExpression;
            }
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
                if (node.HasYield())
                {
                    var bb = new YBlockBuilder();
                    var args = new List<YExpression>();
                    foreach (var item in node.args)
                    {
                        var a = Visit(item);
                        args.Add(bb.ConvertToVariable(a));
                    }
                    bb.AddExpression(Expression.New(node.constructor, args));
                    return bb.Build();

                }
                return base.VisitNew(node);
            }

            protected override Exp VisitField(YFieldExpression yFieldExpression)
            {
                if (yFieldExpression.Target == null)
                    return yFieldExpression;
                var target = Visit(yFieldExpression.Target);
                if (target.HasYield())
                {
                    var bb = new YBlockBuilder();
                    target = bb.ConvertToVariable(target);
                    bb.AddExpression(YExpression.Field(target, yFieldExpression.FieldInfo));
                    return bb.Build();
                }
                return yFieldExpression;
            }

            protected override Exp VisitIndex(YIndexExpression yIndexExpression)
            {
                var hasYield = yIndexExpression.HasYield();
                if(hasYield)
                {
                    var bb = new YBlockBuilder();
                    var target = Visit(yIndexExpression.Target);
                    if (target.HasYield())
                    {
                        target = bb.ConvertToVariable(target);
                    }

                    var args = new List<YExpression>();

                    foreach(var item in yIndexExpression.Arguments)
                    {
                        var e = Visit(item);
                        args.Add(bb.ConvertToVariable(e));
                    }

                    bb.AddExpression(YExpression.Index(target, yIndexExpression.Property, args.ToArray()));
                    return bb.Build();
                }
                return yIndexExpression;
            }

            protected override Exp VisitArrayIndex(YArrayIndexExpression yArrayIndexExpression)
            {
                var target = Visit(yArrayIndexExpression.Target);
                var index = Visit(yArrayIndexExpression.Index);

                var targetHasYield = target.HasYield();
                var indexHasYield = index.HasYield();

                if(targetHasYield || indexHasYield)
                {
                    var bb = new YBlockBuilder();
                    
                    if (targetHasYield)
                    {
                        target = bb.ConvertToVariable(target);
                    }
                    if (indexHasYield)
                    {
                        index = bb.ConvertToVariable(index);
                    }
                    bb.AddExpression(YExpression.ArrayIndex(target, index));
                    return bb.Build();
                }
                return yArrayIndexExpression;
            }

            protected override Expression VisitCall(YCallExpression node)
            {
                if(node.HasYield())
                {

                    // rewrite...
                    var bb = new YBlockBuilder();

                    var target = Visit(node.Target);
                    if (target?.HasYield() ?? false)
                    {
                        target = bb.ConvertToVariable(target);
                    }

                    var args = new List<YExpression>();
                    foreach(var item in node.Arguments)
                    {
                        var a = Visit(item);
                        args.Add(bb.ConvertToVariable(a));
                    }
                    bb.AddExpression(Expression.Call(target, node.Method, args));
                    return bb.Build();
                }

                return node;
            }
        }

    }
}
