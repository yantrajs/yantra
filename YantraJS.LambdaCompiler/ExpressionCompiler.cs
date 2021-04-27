using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;
using YantraJS.Generator;

namespace YantraJS
{
    public interface IMethodRepository
    {
        object Run(int id);
        int RegisterNew(Delegate d);
    }

    public static class ExpressionCompiler
    {

        private static int id = 1;

        internal static string GetUniqueName(string name)
        {
            return $"<YantraJSHidden>{name}<ID>{System.Threading.Interlocked.Increment(ref id)}";
        }


        internal static void InternalCompileToMethod(
            YLambdaExpression exp,
            MethodBuilder builder)
        {

            NestedRewriter nw = new NestedRewriter(exp, new LambdaMethodBuilder(builder));
            exp = nw.Visit(exp) as YLambdaExpression;

            ILCodeGenerator icg = new ILCodeGenerator(builder.GetILGenerator());
            icg.Emit(exp);

            
        }


        public static void CompileToMethod(
            YLambdaExpression lambdaExpression,
            MethodBuilder methodBuilder)
        {
            var exp = LambdaRewriter.Rewrite(lambdaExpression)
                as YLambdaExpression;

            InternalCompileToMethod(exp, methodBuilder);
        }

        internal static MethodInfo Compile(YLambdaExpression expression,
            IMethodRepository methods)
        {
            var exp = LambdaRewriter.Rewrite(expression)
                as YLambdaExpression;

            var runtimeMethodBuilder = new RuntimeMethodBuilder(methods);

            return InternalCompileToMethod(exp, runtimeMethodBuilder);
        }

        public static T Compile<T>(this YLambdaExpression expression)
        {
            var m = Compile(expression, null);
            return (T)(object)m.CreateDelegate(typeof(T));
        }

        public static T CompileWithNestedLambdas<T>(this YLambdaExpression expression)
        {
            var repository = new MethodRepository();

            var outerLambda = YExpression.Lambda(expression.Name + "_outer", expression, new List<YParameterExpression> { 
                YExpression.Parameter(typeof(IMethodRepository))
            });

            var outer = Compile(outerLambda, repository);

            var func = outer.CreateDelegate(typeof(Func<IMethodRepository, object>)) as Func<IMethodRepository, object>;

            return (T)(object)func(repository);
        }


        internal static MethodInfo InternalCompileToMethod(
            YLambdaExpression exp,
            RuntimeMethodBuilder builder)
        {

            NestedRewriter nw = new NestedRewriter(exp, builder);
            exp = nw.Visit(exp) as YLambdaExpression;

            DynamicMethod dm = new DynamicMethod(
                exp.Name,
                exp.Type,
                exp.ParameterTypes, typeof(object), true);

            ILCodeGenerator icg = new ILCodeGenerator(dm.GetILGenerator());
            icg.Emit(exp);

            string value = icg.ToString();

            return dm;
        }

    }

    public class RuntimeMethodBuilder : IMethodBuilder
    {
        private readonly IMethodRepository methods;

        public RuntimeMethodBuilder(IMethodRepository methods)
        {
            this.methods = methods;
        }

        private static Type type = typeof(IMethodRepository);

        private static MethodInfo run
            = type.GetMethod(nameof(IMethodRepository.Run));

        public YExpression Create(string name, YLambdaExpression lambdaExpression)
        {
            if (lambdaExpression.Repository == null)
                throw new NotSupportedException($"Compile with Method Repository");
            var method = ExpressionCompiler.Compile(lambdaExpression, methods);
            var d = method.CreateDelegate(lambdaExpression.DelegateType);
            var id = methods.RegisterNew(d);
            return YExpression.Call(lambdaExpression.Repository, run, YExpression.Constant(id));
        }
    }

    public class MethodRepository : IMethodRepository
    {
        private List<Delegate> delegates = new List<Delegate>();
        public int RegisterNew(Delegate d)
        {
            int i = delegates.Count;
            delegates.Add(d);
            return i;
        }

        public object Run(int id)
        {
            return delegates[id];
        }
    }
}
