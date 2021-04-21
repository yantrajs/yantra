using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace YantraJS
{


    public class BoxParamter
    {
        public Type Type;
        public int Index;
        public ParameterExpression Parameter;
        internal bool Create;
        internal MemberExpression Expression;
        internal Expression Parent;
    }

    public class PendingReplacements
    {
        public Dictionary<ParameterExpression, BoxParamter> Variables { get; }
            = new Dictionary<ParameterExpression, BoxParamter>();

    }

    public class LambdaRewriter: ExpressionVisitor
    {

        public bool Collect = true;

        private ClosureScopeStack stack = new ClosureScopeStack();

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

                ParameterExpression closures = null;

                foreach(var p in n.Parameters)
                {
                    if(top.PendingReplacements.Variables.TryGetValue(p, out var bp))
                    {
                        if (bp == null)
                            continue;
                        if (bp.Create)
                        {
                            localBoxes.Add(bp.Parameter);
                            stmts.Add(Expression.Assign(bp.Parameter, Expression.New(bp.Parameter.Type)));
                            stmts.Add(Expression.Assign(bp.Expression, p ));
                        } else
                        {
                            // probably closure from parent...
                            if (bp.Parent != null)
                            {
                                if (closures == null)
                                {
                                    closures = Expression.Parameter(typeof(Box[]));
                                    stmts.Add(Expression.Assign(closures, 
                                        Expression.NewArrayBounds(typeof(Box), Expression.Constant(top.Length))));
                                }

                                stmts.Add(Expression.Assign(bp.Parent, 
                                        Expression.TypeAs(
                                            Expression.ArrayIndex(closures, Expression.Constant( bp.Index)),
                                            bp.Parent.Type)
                                    ));
                            }
                        }
                    }
                }

                return null;
            }
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            if (Collect)
            {
                stack.Register(node.Variables);
            }
            return base.VisitBlock(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (Collect)
            {
                stack.Access(node);
                return node;
            }

            // second phase... replace...
            if(stack.Top.PendingReplacements.Variables.TryGetValue(node, out var be))
            {
                return base.VisitParameter(be.Parameter);
            }

            return base.VisitParameter(node);
        }

        public static Expression Rewrite<T,TR>(Expression<Func<T, TR>> factory)
        {
            var l = new LambdaRewriter();
            return l.Convert(factory);
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
