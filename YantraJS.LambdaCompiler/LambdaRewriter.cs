#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS
{

    public class LambdaRewriter: YExpressionMapVisitor
    {

        public bool Collect = true;

        private ClosureScopeStack stack = new ClosureScopeStack();
        private readonly YParameterExpression? repository;

        public LambdaRewriter(YParameterExpression? repository)
        {
            this.repository = repository;
        }


        protected override YExpression VisitLambda(YLambdaExpression node)
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

            YExpression PostVisit(YLambdaExpression n, ClosureScopeStack.ClosureScopeItem top)
            {
                List<YExpression> stmts = new List<YExpression>();
                List<YParameterExpression> localBoxes = new List<YParameterExpression>();
                var localClosures 
                    = new List<(YParameterExpression, YParameterExpression)>();

                YParameterExpression? closures = null;

                List<YExpression> closureSetup = new List<YExpression>();

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
                                closures = YExpression.Parameter(typeof(Box[]));
                            }

                            closureSetup.Add(YExpression.Assign(
                                YExpression.ArrayIndex(closures, YExpression.Constant(bp.Index)), 
                                BoxHelper.For(bp.Parent.Type).New(bp.Parent)));

                            stmts.Add(YExpression.Assign(bp.Parameter,
                                    YExpression.TypeAs(
                                        YExpression.ArrayIndex(closures, YExpression.Constant(bp.Index)),
                                        bp.Parameter.Type)
                                ));
                        }
                        else {
                            stmts.Add(YExpression.Assign(bp.Parameter, YExpression.New(bp.Parameter.Type)));

                            var p1 = n.Parameters.FirstOrDefault(x => x == p.Key);
                            if (p1 != null)
                            {
                                stmts.Add(YExpression.Assign(bp.Expression, p1));
                            }
                        }
                    }

                }

                var selfRepository = repository as YExpression;

                // forece repository access...
                if(repository != null)
                {
                    selfRepository = VisitParameter(repository);
                }

                var body = Visit(n.Body);

                if(closures == null)
                {
                    if(stmts.Count == 0)
                    {
                        return n;
                    }

                    stmts.Add(body);

                    return YExpression.Lambda( n.Name, YExpression.Block(localBoxes, body), n.Parameters);
                }

                // curry....
                if (stmts.Count > 0 || localBoxes.Count > 0)
                {
                    stmts.Add(body);
                    body = YExpression.Block(localBoxes, stmts);
                }

                closureSetup.Insert(0, YExpression.Assign(closures,
                    YExpression.NewArrayBounds(typeof(Box), YExpression.Constant(closureSetup.Count))));


                return CurryHelper.Create(n.Name ?? "unnamed", closureSetup, closures, n.Parameters, body, selfRepository);
            }
        }

        protected override YExpression VisitBlock(YBlockExpression node)
        {
            if (Collect)
            {
                // variables will be pushed.. so we dont need them...
                stack.Register(node.Variables);
                return VisitBlock(node);
            }

            return YExpression.Block(null, node.Expressions.Select(x => Visit(x)).ToArray());
        }

        protected override YExpression VisitParameter(YParameterExpression node)
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

        public static YExpression Rewrite(YLambdaExpression convert)
        {
            var l = new LambdaRewriter(convert.Parameters.FirstOrDefault(x => x.Type == typeof(IMethodRepository)));
            return l.Convert(convert);
        }

        private YExpression Convert(YExpression exp)
        {
            using (var scope = stack.Push(exp)) {
                exp = Visit(exp);
                Collect = false;
                return Visit(exp);
            }
        }
    }
}
