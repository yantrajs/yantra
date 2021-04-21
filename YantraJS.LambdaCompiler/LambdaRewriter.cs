using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                                closureSetup.Add(Expression.Assign(closures,
                                    Expression.NewArrayBounds(typeof(Box), Expression.Constant(top.Length))));
                            }

                            closureSetup.Add(Expression.Assign(
                                Expression.ArrayAccess(closures, Expression.Constant(bp.Index)), 
                                bp.Parent));

                            stmts.Add(Expression.Assign(bp.Parameter,
                                    Expression.TypeAs(
                                        Expression.ArrayIndex(closures, Expression.Constant(bp.Index)),
                                        bp.Parent.Type)
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

                List<ParameterExpression> plist = new List<ParameterExpression>(n.Parameters.Count+1) { closures };
                plist.AddRange(n.Parameters);

                // curry....
                if (stmts.Count > 0 || localBoxes.Count > 0)
                {
                    stmts.Add(body);
                    body = Expression.Block(localBoxes, stmts);
                }

                return CurryHelper.Create(closureSetup, closures, plist, body);
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
