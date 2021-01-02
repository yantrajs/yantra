using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static YantraJS.Core.LinqExpressions.Generators.ClrGenerator;

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
        private static MethodInfo _call = type.GetMethod(nameof(ClrGenerator.Call));
        private static MethodInfo _if = type.GetMethod(nameof(ClrGenerator.If));
        private static MethodInfo _unary = type.GetMethod(nameof(ClrGenerator.Unary));
        private static MethodInfo _loop = type.GetMethod(nameof(ClrGenerator.Loop));
        private static MethodInfo _goto = type.GetMethod(nameof(ClrGenerator.Goto));
        private static MethodInfo _yield = type.GetMethod(nameof(ClrGenerator.Yield));
        private static MethodInfo _memberAccess = type.GetMethod(nameof(ClrGenerator.MemberAccess));
        private static MethodInfo _build = type.GetMethod(nameof(ClrGenerator.Build));
        private static MethodInfo _assign = type.GetMethod(nameof(ClrGenerator.Assign));
        private static MethodInfo _switch = type.GetMethod(nameof(ClrGenerator.Switch));
        private static MethodInfo _tryCatchFinally = type.GetMethod(nameof(ClrGenerator.TryCatchFinally));
        private static MethodInfo _tryCatch = type.GetMethod(nameof(ClrGenerator.TryCatch));
        private static MethodInfo _tryFinally = type.GetMethod(nameof(ClrGenerator.TryFinally));

        private static Type caseBlockType = typeof(CaseBody);
        private static ConstructorInfo newCaseBlock = caseBlockType.Constructor(typeof(object[]), typeof(Func<object>));

        private static Expression ToCaseExpression(SwitchCase @case)
        {
            var body = Expression.Lambda(typeof(Func<object>), @case.Body.AsObject());
            var tests = Expression.NewArrayInit(typeof(object), @case.TestValues);
            return Expression.New(newCaseBlock, tests, body);
        }

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

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (!node.ShouldBreak())
                return node;
            return node.ToLambda();
        }

        protected override Expression VisitNew(NewExpression node)
        {
            if (!node.ShouldBreak())
                return node;

            return ExecuteCall(node.Arguments, a => node.Update(a));
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!node.ShouldBreak())
                return node;

            return ExecuteCall(node.Arguments, a => node.Update(node.Object,a));
        }

        private Expression ExecuteCall(IEnumerable<Expression> arguments, Func<List<Expression>, Expression> transform)
        {
            // break every parameter...
            ParameterExpression peList = Expression.Parameter(typeof(object[]));
            // cast to native type...
            List<Expression> argList = new List<Expression>();

            List<Expression> lambaList = new List<Expression>();

            int i = 0;
            foreach (var a in arguments)
            {
                var al = Visit(a).ToLambda();
                var pe = Expression.ArrayIndex(peList,Expression.Constant(i++));
                
                argList.Add(a.Type.IsValueType
                    ? Expression.Convert(pe, a.Type)
                    : Expression.TypeAs(pe, a.Type));
                if (a.Type.IsValueType)
                {
                    lambaList.Add(al);
                }
                else
                {
                    lambaList.Add(Expression.Lambda(typeof(Func<object>), a));
                }
            }

            var newNode =
                Expression.Lambda(typeof(Func<object[], object>),
                transform(argList).AsObject(), peList);



            return Expression.Call(generator, _call,
                Expression.NewArrayInit(typeof(Func<object>), lambaList),
                newNode);
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
                //if (n.Type == typeof(Func<object>))
                //    return n;
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

        protected Expression  VisitAssign(BinaryExpression node)
        {
            var p = Expression.Parameter(node.Left.Type);
            var left = Expression.Lambda(typeof(Action<>).MakeGenericType(p.Type),
                node.Update(node.Left, node.Conversion,p),p);
            var right = Convert(node.Right);
            return Expression.Call(generator, _assign.MakeGenericMethod(p.Type), left, right);
        }


        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (!node.ShouldBreak())
                return node;
            var nodeLeft = node.Left;

            switch (node.NodeType)
            {
                case ExpressionType.Assign:
                    return VisitAssign(node);
            }

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

        protected Expression VisitSwitch(SwitchExpression @switch, LabelExpression label)
        {
            var @break = labels[label.Target];
            var target = Convert(@switch.SwitchValue);
            var @default = Convert(@switch.DefaultBody);

            var @cases = @switch.Cases.Select(
                x => ToCaseExpression(x)
                ).ToList();

            var plist = new List<Expression>() {
                target,
                Expression.Constant(@break),
                Expression.NewArrayInit(typeof(CaseBody), @cases),
                @default
            };


            return Expression.Call(generator, _switch, plist);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            if(node.Expressions.Count == 2)
            {
                if(node.Expressions[0] is SwitchExpression @switch)
                {
                    if(node.Expressions[1] is LabelExpression label)
                    {
                        if (@switch.ShouldBreak())
                        {
                            return VisitSwitch(@switch, label);
                        }
                    }
                }
            }

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

        protected override Expression VisitTry(TryExpression node)
        {
            if (!node.ShouldBreak())
                return node;
            Expression @catch = null;
            if (node.Handlers.Any())
            {
                var cb = node.Handlers.Single();
                var pe = Expression.Parameter(typeof(Exception));
                @catch = Expression.Lambda(typeof(Func<Exception,object>), 
                    Expression.Block(
                        Expression.Assign(cb.Variable, pe),
                        cb.Body
                    ), pe);
            }
            var @try = Convert(node.Body);
            var @finally = Convert(node.Finally);
            if (node.Finally != null)
            {
                if (@catch != null)
                    return Expression.Call(generator, _tryCatchFinally, @try, @catch, @finally);
                return Expression.Call(generator, _tryFinally, @try, @finally);
            }

            return Expression.Call(generator, _tryCatch, @try, @catch);
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            if (!node.ShouldBreak())
                return node;
            // return base.VisitSwitch(node);
            throw new NotSupportedException();
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (!node.ShouldBreak())
                return node;
            var returnType = node.Expression.Type;
            var t = typeof(Func<,>).MakeGenericType(returnType, typeof(object));
            var pe = Expression.Parameter(node.Expression.Type, "member");
            var updatedNode = node.Update(Expression.Convert(pe, returnType));
            var body = Expression.Lambda(t, updatedNode.AsObject(), pe);
            return Expression.Call(generator,
                _memberAccess.MakeGenericMethod(node.Expression.Type),
                Visit(node.Expression),
                body);
        }
    }
}
