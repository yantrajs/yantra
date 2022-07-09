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
            Closures = new Dictionary<YParameterExpression, (YParameterExpression local, YExpression value, int index, int argIndex)>(ReferenceEqualityComparer.Instance);

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
            bool isBox = typeof(Box).IsAssignableFrom(pe.Type);
            var boxType = isBox ? pe.Type : BoxHelper.For(pe.Type).BoxType;
            var converted = YExpression.Parameter(boxType, pe.Name + "`");
            YExpression valueField = isBox ? converted : YExpression.Field(converted, "Value");
            Closures[pe] = (converted, valueField, Inputs.Count, -1);
            Inputs.Add(s);
            return converted;
        }

        internal YParameterExpression Convert(YParameterExpression pe)
        {
            if (Closures.TryGetValue(pe, out var value))
                return value.local;
            bool isBox = typeof(Box).IsAssignableFrom(pe.Type);
            var boxType = isBox ? pe.Type : BoxHelper.For(pe.Type).BoxType;
            var converted = YExpression.Parameter(boxType, pe.Name + "`");
            YExpression valueField = isBox ? converted : YExpression.Field(converted, "Value");
            var argIndex = Array.IndexOf(lambda.Parameters, pe);
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
                    var ve = variables.GetFastEnumerator();
                    while(ve.MoveNext(out var v))
                    {
                        Variables.Remove(v);
                    }
                });
            }
        }

        private ScopedStack<Scope> lambdaStack = new ScopedStack<Scope>();
        private YLambdaExpression RootExpression;

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
            if (node != RootExpression)
            {
                node.SetupAsClosure();
            }
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
            l.RootExpression = convert;
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

}
