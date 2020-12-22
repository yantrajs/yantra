using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YantraJS.Core.LinqExpressions.Generators
{
    public class YieldRewriter : ExpressionVisitor
    {

        private static Type type = typeof(ClrGenerator);

        private static MethodInfo _block = type.GetMethod(nameof(ClrGenerator.Block));
        private static MethodInfo _binary = type.GetMethod(nameof(ClrGenerator.Binary));
        private static MethodInfo _if = type.GetMethod(nameof(ClrGenerator.If));
        private static MethodInfo _unary = type.GetMethod(nameof(ClrGenerator.Unary));

        List<ParameterExpression> lifedVariables = new List<ParameterExpression>();

        public ParameterExpression generator;

        private bool split = false;

        public static Expression Rewrite(Expression body, ParameterExpression generator)
        {
            return (new YieldRewriter(generator)).Visit(body);
        }

        public YieldRewriter(ParameterExpression generator)
        {
            this.generator = generator;
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
                return node;
            return base.Visit(node);
        }

        private Expression ConvertTyped(Expression node)
        {
            if (node == null)
                return Expression.Constant(null);
            if (node.HasYield())
            {
                return Visit(node).CastAs(node.Type).ToLambda();
            }
            return node.ToLambda();
        }

        private Expression Convert(Expression node)
        {
            if (node == null)
                return Expression.Constant(null);
            if (node.HasYield())
            {
                return Visit(node).ToLambda();
            }
            return node.ToLambda();
        }



        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (!split)
                return node;
            var left = ConvertTyped(node.Left);
            var right = ConvertTyped(node.Right);
            return ClrGeneratorBuilder.Binary(generator, left, right, node);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            if (!split)
                return node;
            return Expression.Call(generator,
                ClrGeneratorBuilder._if,
                ConvertTyped(node.Test),
                Convert(node.IfTrue),
                Convert(node.IfFalse));
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (!split)
                return node;
            var converted = ConvertTyped(node.Operand);
            var p = Expression.Parameter(node.Operand.Type);
            var m = _unary.MakeGenericMethod(p.Type);
            var body = Expression.Lambda(node.Update(p),p);
            return Expression.Call(generator, m, converted, body);
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            if (!split)
                return node;
            return base.VisitLoop(node);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            lifedVariables.AddRange(node.Variables);

            VMBlock block = new VMBlock();
            if (split)
            {
                block.Break();
            }
            if (lifedVariables.Count > 0)
            {
                foreach (var lv in lifedVariables)
                {
                    block.Add(Expression.Assign(lv, Expression.Constant(null, lv.Type)));
                }
            }
            foreach (var e in node.Expressions)
            {
                var child = e;
                if (split || YieldFinder.ContainsYield(child))
                {
                    block.Break();
                    try { 
                        split = true;
                        child = Visit(child);
                    } finally
                    {
                        split = false;
                    }
                }
                if (split)
                {
                    child = Visit(child);
                }
                block.Add(child);
            }
            if (lifedVariables.Count > 0)
            {
                foreach (var lv in lifedVariables)
                {
                    block.Add(Expression.Assign(lv, Expression.Constant(null, lv.Type)));
                }
            }

            return block.ToExpression(generator);
        }
    }
}
