using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace YantraJS
{

    public class Box
    {

    }

    public class Box<T>: Box
    {
        public T Value;
    }

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
                List<ParameterExpression> bp = new List<ParameterExpression>();
                List<Expression> stmts = new List<Expression>();
                List<ParameterExpression> localClosures = new List<ParameterExpression>();
                foreach(var t in top.PendingReplacements.Variables)
                {
                    var bv = t.Value;
                    if (bv == null)
                        continue;
                    if(bv.Create)
                    {
                        localClosures.Add(bv.Parameter);
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
