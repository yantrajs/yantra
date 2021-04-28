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

                var selfRepository = repository as YExpression;

                // forece repository access...
                if (repository != null)
                {
                    selfRepository = VisitParameter(repository);
                }

                var body = Visit(n.Body);


                foreach (var p in top.PendingReplacements.Variables)
                {
                    var bp = p.Value;
                    if (bp == null)
                        continue;
                    localBoxes.Add(bp.Parameter);
                    if (bp.Parent != null)
                    {
                        if(bp.Parent.Type == typeof(IMethodRepository))
                        {
                            selfRepository = bp.Parent;
                        }

                        if (closures == null)
                        {
                            closures = YExpression.Parameter(typeof(Box[]));
                        }
                        stmts.Add(YExpression.Assign(bp.Parameter, YExpression.ArrayIndex(closures, YExpression.Constant(bp.Index))));
                        closureSetup.Add(bp.ParentParameter);
                    }
                    if (bp.Create)
                    {
                        if (bp.Parent == null)
                        {
                            stmts.Add(YExpression.Assign(bp.Parameter, YExpression.New(bp.Parameter.Type, p.Key)));
                        }
                    }

                }

                if(closures == null)
                {
                    if(stmts.Count == 0 && body == n.Body)
                    {
                        return n;
                    }

                    stmts.Add(body);

                    return YExpression.InlineLambda( n.Name, YExpression.Block(localBoxes, stmts), n.Parameters, this.repository);
                }

                // curry....
                if (stmts.Count > 0 || localBoxes.Count > 0)
                {
                    stmts.Add(body);
                    body = YExpression.Block(localBoxes, stmts);
                }

                var x = CurryHelper.Create(n.Name ?? "unnamed", closureSetup, closures, n.Parameters, body, selfRepository);
                return x;
            }
        }

        protected override YExpression VisitBlock(YBlockExpression node)
        {
            if (Collect)
            {
                // variables will be pushed.. so we dont need them...
                stack.Register(node.Variables);
                // node = new YBlockExpression(null, node.Expressions);
            }

            return base.VisitBlock(node);
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
                var  r = Visit(exp);
                Collect = false;
                var t = Visit(r);
                return t;
            }
        }
    }
}
