using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
                    if (t == null)
                        return 0;
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
        private static MethodInfo _yield = type.GetMethod(nameof(ClrGenerator.Yield));
        private static MethodInfo _build = type.GetMethod(nameof(ClrGenerator.Build));

        List<ParameterExpression> lifedVariables = new List<ParameterExpression>();

        private __Labels labels = new __Labels(8);


        public ParameterExpression generator;

        public static Expression Rewrite(
            Expression body,
            ParameterExpression pe,
            params ParameterExpression[] generators)
        {
            // var lambdaBody = (new YieldRewriter(generator)).Visit(body);
            // return Expression.Lambda(lambdaBody, generator);

            YieldFinder.MarkYield(body);

            var yr = new YieldRewriter(pe);
            var l = new List<ParameterExpression>();
            l.AddRange(generators);
            Expression b = yr.Visit(body);
            b = Expression.Call(pe, _build, b);
            l.AddRange(yr.lifedVariables);
            b = Expression.Block(l, b);
            return b;
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

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (!node.ShouldBreak())
                return node;
            return node.ToLambda();
        }

        protected override Expression VisitExtension(Expression node)
        {
            if (node is YieldExpression @yield)
            {
                var arg = yield.Argument.AsObject().ToLambda();
                return Expression.Call(generator, _yield, arg);
            }
            return base.VisitExtension(node);
        }

        private Expression ConvertTyped(Expression node)
        {
            if (node == null)
                return Expression.Constant(null);
            if (node.ShouldBreak())
            {
                var n = Visit(node);
                if (n.Type == typeof(Func<object>))
                    return n;
                return n.ToLambda(node.Type);
            }
            return Visit(node).ToLambda(node.Type);
        }

        private Expression Convert(Expression node)
        {
            if (node == null)
                return Expression.Constant(null,typeof(object));
            if (node.ShouldBreak())
            {
                return Visit(node).AsObject().ToLambda();
            }
            return node.AsObject().ToLambda();
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            return null;
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return null;
        }


        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (!node.ShouldBreak())
                return node;
            var nodeLeft = node.Left;
            ParameterExpression leftParemeter = null;
            Expression target = node.Left;
            var rightParameter = Expression.Parameter(node.Left.Type);
            switch ((nodeLeft.NodeType, nodeLeft))
            {
                case (ExpressionType.MemberAccess, MemberExpression me):
                    leftParemeter = Expression.Parameter(me.Expression.Type);
                    nodeLeft = me.Update(leftParemeter);
                    target = me.Expression;
                    break;
                case (ExpressionType.Index, IndexExpression ie):
                    leftParemeter = Expression.Parameter(ie.Object.Type);
                    nodeLeft = ie.Update(leftParemeter, ie.Arguments);
                    target = ie.Object;
                    break;
                default:
                    leftParemeter = Expression.Parameter(node.Left.Type);
                    break;
            }
            var left = ConvertTyped(target);
            var right = ConvertTyped(node.Right);
            var final = Expression.Lambda(node.Update(nodeLeft, node.Conversion, rightParameter), leftParemeter, rightParameter);
            return Expression.Call(generator, 
                _binary.MakeGenericMethod(target.Type,rightParameter.Type), 
                left, 
                right, 
                final);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            if (!node.ShouldBreak())
                return node;
            return Expression.Call(generator,
                ClrGeneratorBuilder._if,
                ConvertTyped(node.Test),
                Convert(node.IfTrue),
                Convert(node.IfFalse));
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (!node.ShouldBreak())
                return node;
            var converted = ConvertTyped(node.Operand);
            var p = Expression.Parameter(node.Operand.Type);
            var m = _unary.MakeGenericMethod(p.Type);
            var body = Expression.Lambda(node.Update(p).AsObject(),p);
            return Expression.Call(generator, m, converted, body);
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            if (!node.ShouldBreak())
                return node;
            var @break = labels[node.BreakLabel];
            var @continue = labels[node.ContinueLabel];
            var @block = Convert(node.Body);
            return Expression.Call(generator,_loop, @block, Expression.Constant(@break), Expression.Constant(@continue));
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            if (!node.ShouldBreak())
                return node;

            var target = labels[node.Target];
            return Expression.Call(generator, _goto, Expression.Constant(target));
        }

        protected override Expression VisitBlock(BlockExpression node)
        {

            node = node.Reduce() as BlockExpression;

            lifedVariables.AddRange(node.Variables);

            VMBlock block = new VMBlock();
            //if (lifedVariables.Count > 0)
            //{
            //    foreach (var lv in lifedVariables)
            //    {
            //        block.Add(Expression.Assign(lv, Expression.Constant(null, lv.Type)));
            //    }
            //}
            foreach (var e in node.Expressions)
            {
                var child = e;
                if (e.ShouldBreak()) {
                    block.AddYield(Visit(e));
                } else {
                    block.Add(Visit(child));
                }
            }
            //if (lifedVariables.Count > 0)
            //{
            //    foreach (var lv in lifedVariables)
            //    {
            //        block.Add(Expression.Assign(lv, Expression.Constant(null, lv.Type)));
            //    }
            //}

            return block.ToExpression(generator);
        }
    }
}
