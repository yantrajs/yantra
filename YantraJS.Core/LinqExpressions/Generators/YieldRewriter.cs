using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace YantraJS.Core.LinqExpressions.Generators
{
    

    public class YieldRewriter : ExpressionVisitor
    {

        struct __Labels {
            uint lastID;
            Dictionary<LabelTarget, uint> labels;
            public __Labels(int i)
            {
                labels = new Dictionary<LabelTarget, uint>(i);
                lastID = 1;
            }

            public uint this[LabelTarget t] { 
                get
                {
                    if (labels.TryGetValue(t, out var i))
                        return i;
                    i = lastID++;
                    labels[t] = i;
                    return i;
                }
            }

        }

        private static Type type = typeof(ClrGenerator);

        private static MethodInfo _block = type.GetMethod(nameof(ClrGenerator.Block));
        private static MethodInfo _binary = type.GetMethod(nameof(ClrGenerator.Binary));
        private static MethodInfo _if = type.GetMethod(nameof(ClrGenerator.If));
        private static MethodInfo _unary = type.GetMethod(nameof(ClrGenerator.Unary));
        private static MethodInfo _loop = type.GetMethod(nameof(ClrGenerator.Loop));
        private static MethodInfo _goto = type.GetMethod(nameof(ClrGenerator.Goto));

        List<ParameterExpression> lifedVariables = new List<ParameterExpression>();

        private __Labels labels = new __Labels(8);


        public ParameterExpression generator;

        private bool split = false;

        public static Expression Rewrite(Expression body, ParameterExpression generator)
        {
            var lambdaBody = (new YieldRewriter(generator)).Visit(body);
            return Expression.Lambda(lambdaBody, generator);
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
            return ClrGeneratorBuilder.Binary(generator, left, node.Left.Type, right, node.Right.Type, node);
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
            var @break = labels[node.BreakLabel];
            var @continue = labels[node.ContinueLabel];
            var @block = Convert(node.Body);
            return Expression.Call(generator,_loop, @block, Expression.Constant(@break), Expression.Constant(@continue));
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            if (!split)
                return node;
            var target = labels[node.Target];
            return Expression.Call(generator, _goto, Expression.Constant(target));
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
