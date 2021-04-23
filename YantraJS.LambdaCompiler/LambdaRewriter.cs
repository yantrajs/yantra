#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace YantraJS
{

    public class LambdaRewriter: ExpressionVisitor
    {

        public bool Collect = true;

        private ClosureScopeStack stack = new ClosureScopeStack();
        private readonly IMethodBuilder? methodBuilder;

        public LambdaRewriter(IMethodBuilder? methodBuilder = null)
        {
            this.methodBuilder = methodBuilder;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (Collect)
            {

                if (node == stack.Top.Expression)
                {
                    // register parameters...

                    stack.Register(node.Parameters);

                    return base.VisitLambda(node);
                }
                using (var scope = stack.Push(node))
                {
                    stack.Register(node.Parameters);
                    return base.VisitLambda(node);
                }
            }

            if(node == stack.Top.Expression)
            {
                return PostVisit(node, stack.Top);
            }

            using (var scope = stack.Push(node))
            {
                return PostVisit(node, scope);
            }

            Expression PostVisit(LambdaExpression n, ClosureScopeStack.ClosureScopeItem top)
            {
                List<Expression> stmts = new List<Expression>();
                List<ParameterExpression> localBoxes = new List<ParameterExpression>();
                var localClosures 
                    = new List<(ParameterExpression, ParameterExpression)>();

                ParameterExpression? closures = null;

                List<Expression> closureSetup = new List<Expression>();

                foreach(var p in top.PendingReplacements.Variables)
                {
                    var bp = p.Value;
                    if (bp == null)
                        continue;
                    if (bp.Create || bp.Parameter != null)
                    {

                        localBoxes.Add(bp.Parameter);
                        if (bp.Parent != null)
                        {
                            if (closures == null)
                            {
                                closures = Expression.Parameter(typeof(Box[]));
                            }

                            closureSetup.Add(Expression.Assign(
                                Expression.ArrayAccess(closures, Expression.Constant(bp.Index)), 
                                BoxHelper.For(bp.Parent.Type).New(bp.Parent)));

                            stmts.Add(Expression.Assign(bp.Parameter,
                                    Expression.TypeAs(
                                        Expression.ArrayIndex(closures, Expression.Constant(bp.Index)),
                                        bp.Parameter.Type)
                                ));
                        }
                        else {
                            stmts.Add(Expression.Assign(bp.Parameter, Expression.New(bp.Parameter.Type)));

                            var p1 = n.Parameters.FirstOrDefault(x => x == p.Key);
                            if (p1 != null)
                            {
                                stmts.Add(Expression.Assign(bp.Expression, p1));
                            }
                        }
                    }

                }

                var body = Visit(n.Body);

                if(closures == null)
                {
                    if(stmts.Count == 0)
                    {
                        return n;
                    }

                    stmts.Add(body);

                    return Expression.Lambda(Expression.Block(localBoxes, body), n.Parameters);
                }

                // curry....
                if (stmts.Count > 0 || localBoxes.Count > 0)
                {
                    stmts.Add(body);
                    body = Expression.Block(localBoxes, stmts);
                }

                closureSetup.Insert(0, Expression.Assign(closures,
                    Expression.NewArrayBounds(typeof(Box), Expression.Constant(closureSetup.Count))));


                return CurryHelper.Create(n.Name ?? "unnamed", closureSetup, closures, n.Parameters, body);
            }
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            if (Collect)
            {
                // variables will be pushed.. so we dont need them...
                stack.Register(node.Variables);
                return VisitBlock(node);
            }

            return Expression.Block(node.Expressions.Select(x => Visit(x)));
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (Collect)
            {
                stack.Access(node);
                return node;
            }

            // second phase... replace...
            //if(stack.Top.PendingReplacements.Variables.TryGetValue(node, out var be))
            //{
            //    return base.VisitParameter(be.Parameter);
            //}

            return stack.Access(node);
        }

        public static Expression Rewrite<T,TR>(Expression<Func<T, TR>> factory)
        {
            var l = new LambdaRewriter();
            return l.Convert(factory);
        }

        public static Expression Rewrite(Expression convert, IMethodBuilder? methodBuilder)
        {
            var l = new LambdaRewriter(methodBuilder);
            return l.Convert(convert);
        }

        private Expression Convert(Expression exp)
        {
            using (var scope = stack.Push(exp)) {
                exp = Visit(exp);
                Collect = false;
                return Visit(exp);
            }
        }
    }
}
