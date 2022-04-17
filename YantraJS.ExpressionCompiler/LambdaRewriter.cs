#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS
{
    public static class ClosureRepositoryExtensions
    {
        public static ClosureRepository GetClosureRepository(this YLambdaExpression lambda)
        {
            return ClosureRepository.For(lambda);
        }
    }

    public class ClosureRepository
    {
        private static System.Runtime.CompilerServices.ConditionalWeakTable<YLambdaExpression, ClosureRepository> cache =
            new System.Runtime.CompilerServices.ConditionalWeakTable<YLambdaExpression, ClosureRepository>();

        public readonly Dictionary<YParameterExpression, (YParameterExpression local, YExpression value, int index, int argIndex)>
            Closures = new Dictionary<YParameterExpression, (YParameterExpression local, YExpression value, int index, int argIndex)>();

        public List<YParameterExpression> Inputs 
            = new List<YParameterExpression>();

        private YLambdaExpression lambda;

        protected ClosureRepository(YLambdaExpression lambda)
        {
            this.lambda = lambda;
        }

        public static ClosureRepository For(YLambdaExpression lambda)
        {
            if (cache.TryGetValue(lambda, out var value))
                return value;
            value = new ClosureRepository(lambda);
            cache.Add(lambda, value);
            return value;
        }

        internal bool TryGet(YParameterExpression pe, out YExpression exp)
        {
            if (Closures.TryGetValue(pe, out var ve))
            {
                exp = ve.value;
                return true;
            }
            exp = default!;
            return false;
        }

        internal YParameterExpression Setup(YParameterExpression pe, Func<YParameterExpression> source)
        {
            if (Closures.TryGetValue(pe, out var value))
                return value.local;
            var s = source();
            var boxType = BoxHelper.For(pe.Type).BoxType;
            var converted = YExpression.Parameter(boxType, pe.Name + "`");
            var valueField = YExpression.Field(converted, "Value");
            Closures[pe] = (converted, valueField, Inputs.Count, -1);
            Inputs.Add(s);
            return converted;
        }

        internal YParameterExpression Convert(YParameterExpression pe)
        {
            if (Closures.TryGetValue(pe, out var value))
                return value.local;
            var boxType = BoxHelper.For(pe.Type).BoxType;
            var converted = YExpression.Parameter(boxType, pe.Name + "`");
            var valueField = YExpression.Field(converted, "Value");
            var argIndex = -1;
            if (lambda.This == pe)
            {
                argIndex = 0;
            } else
            {
                argIndex = Array.IndexOf(lambda.Parameters, pe) + 1;
            }
            Closures[pe] = (converted, valueField, -1, argIndex);
            return converted;
        }
    }


    public class LambdaRewriter: YExpressionMapVisitor
    {
        public class Scope
        {
            public readonly YLambdaExpression Root;
            public readonly List<YParameterExpression> Variables = new List<YParameterExpression>();

            public Scope(YLambdaExpression exp)
            {
                this.Root = exp;
                Variables.AddRange(exp.Parameters);
            }

            public static implicit operator Scope(YLambdaExpression e) => new Scope(e);

            internal IDisposable Register(IFastEnumerable<YParameterExpression> variables)
            {
                Variables.AddRange(variables);
                return new DisposableAction(() => { 
                    foreach(var v in variables)
                    {
                        Variables.Remove(v);
                    }
                });
            }
        }

        private ScopedStack<Scope> lambdaStack = new ScopedStack<Scope>();

        public Scope Root => lambdaStack.TopItem;
        
        public LambdaRewriter()
        {

        }

        

        protected override YExpression VisitLambda(YLambdaExpression node)
        {
            /// we will not mark nested lambda as relay for two reasons
            /// 1.  In case of Runtime Execution, IMethodRepository will be
            ///     available as global static variable to directly run and
            ///     register the method.
            /// 2.  In case of Assembly builder, there is no need to maintain
            ///     global repository as AssemblyBuilder will become Method 
            ///     Repository
            using var scope = lambdaStack.Push(node);
            if (node.This != null)
            {
                Root.Register(node.This.AsSequence());
            }
            Root.Register(node.Parameters.AsSequence());
            return base.VisitLambda(node);
        }

        protected override YExpression VisitBlock(YBlockExpression yBlockExpression)
        {
            using var scope = Root.Register(yBlockExpression.Variables);
            return base.VisitBlock(yBlockExpression);
        }

        protected override YExpression VisitParameter(YParameterExpression yParameterExpression)
        {
            CheckForClosure(this.lambdaStack.Top, yParameterExpression);
            return base.VisitParameter(yParameterExpression);
        }

        private YParameterExpression CheckForClosure(ScopedStack<Scope>.ScopedItem current, YParameterExpression pe, bool setup = false)
        {
            if (current.Item.Variables.Contains(pe))
            {
                if (setup)
                {
                    return current.Item.Root.GetClosureRepository().Convert(pe);
                }
                return pe;
            }
            var parent = current.Parent;
            if (parent == null)
                throw new InvalidProgramException();

            var repository = current.Item.Root.GetClosureRepository();
            return repository.Setup(pe, () => CheckForClosure(parent,pe,true));
        }

        public static YExpression Rewrite(YLambdaExpression convert)
        {
            var l = new LambdaRewriter();
            l.Visit(convert);   
            return convert;
            //var l = new LambdaRewriter(
            //    convert.This ?? convert.Parameters.FirstOrDefault(x => x.Type == typeof(IMethodRepository)));
            //return l.Convert(convert);
        }

        //private YExpression Convert(YLambdaExpression exp)
        //{
        //    using (var scope = stack.Push(exp))
        //    {
        //        var r = Visit(exp);
        //        Collect = false;
        //        var t = Visit(r);
        //        return t;
        //    }
        //}
    }

    public class OldLambdaRewriter: YExpressionMapVisitor
    {

        public bool Collect = true;

        private ClosureScopeStack stack = new ClosureScopeStack();
        private readonly YParameterExpression? repository;

        public OldLambdaRewriter(YParameterExpression? repository)
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

                    stack.Top.Register(node);

                    return base.VisitLambda(node);
                }
                using (var scope = stack.Push(node))
                {
                    stack.Top.Register(node);
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
                var stmts = new Sequence<YExpression>();
                var localBoxes = new Sequence<YParameterExpression>();
                var localClosures 
                    = new Sequence<(YParameterExpression, YParameterExpression)>();

                YParameterExpression? closures = null;

                var closureSetup = new Sequence<YExpression>();

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
                        // this is for binding outer lambda's parameter 
                        // to relay
                        if (repository != null && repository.Type.IsAssignableFrom(bp.Parent.Type))
                        {
                            selfRepository = bp.Parent;
                        }

                        if (closures == null)
                        {
                            closures = YExpression.Parameter(typeof(Closures), "thisClosure");
                        }
                        stmts.Add(YExpression.Assign(bp.Parameter, YExpression.ArrayIndex( 
                            YExpression.Field( closures, Closures.boxesField), 
                            YExpression.Constant(bp.Index))));
                        closureSetup.Add(bp.ParentParameter);
                    }
                    if (bp.Create)
                    {
                        if (bp.Parent == null)
                        {
                            if (n.Parameters.Contains(p.Key) || p.Key == n.This)
                            {
                                stmts.Add(YExpression.Assign(bp.Parameter, YExpression.New(bp.Parameter.Type, p.Key)));
                            }
                            else
                            {
                                stmts.Add(YExpression.Assign(bp.Parameter, YExpression.New(bp.Parameter.Type)));
                            }
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

                    return YExpression.InlineLambda( 
                        n.Type,
                        n.Name, 
                        YExpression.Block(localBoxes, stmts), 
                        repository!,
                        n.Parameters,
                        selfRepository,
                        n.ReturnType);
                }

                // curry....
                if (stmts.Count > 0 || localBoxes.Count > 0)
                {
                    stmts.Add(body);
                    body = YExpression.Block(localBoxes, stmts);
                }

                var x = Relay(
                    n.Name, 
                    closureSetup, 
                    closures, 
                    n.Parameters, 
                    body, 
                    selfRepository,
                    n.ReturnType,
                    n.Type);
                return x;
            }
        }

        protected override YExpression VisitRelay(YRelayExpression relayExpression)
        {
            return relayExpression;
        }

        public static YExpression Relay(
            in FunctionName name,
            IFastEnumerable<YExpression> closures,
            YParameterExpression c,
            YParameterExpression[] parameters,
            YExpression body,
            YExpression? repository,
            Type returnType,
            Type delegateType
            )
        {
            var lambda = YExpression.InlineLambda(delegateType, name, body, c, parameters, repository, returnType);

            return YExpression.Relay(closures, lambda);
        }

        protected override YExpression VisitBlock(YBlockExpression node)
        {
            if (Collect)
            {
                // variables will be pushed.. so we dont need them...
                stack.Top.Register(node.Variables);
                // node = new YBlockExpression(null, node.Expressions);
                return base.VisitBlock(node);
            }

            // let us find out lifted variables...

            var list = new Sequence<YParameterExpression>(node.Variables.Count);
            var statements = new Sequence<YExpression>(node.Expressions.Count);
            foreach (var (e, p) in node.Variables.Select(v => stack.AccessParameter(v))) { 
                if(e == p)
                {
                    list.Add(p);
                    continue;
                }
            }

            foreach(var s in node.Expressions)
            {
                statements.Add(Visit(s));
            }

            return new YBlockExpression(list, statements);
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

        //public static YExpression Rewrite(YLambdaExpression convert)
        //{
        //    var l = new LambdaRewriter(
        //        convert.This ?? convert.Parameters.FirstOrDefault(x => x.Type == typeof(IMethodRepository)));
        //    return l.Convert(convert);
        //}

        private YExpression Convert(YLambdaExpression exp)
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
