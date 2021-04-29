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

        public static T CompileInAssembly<T>(this YLambdaExpression exp)
        {
            AssemblyName name = new AssemblyName("demo");

            // lets generate and save...
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);
            // string fileName = System.IO.Path.GetFileNameWithoutExtension(location);
            // name.CodeBase = filePath;
            var mm = ab.DefineDynamicModule("JSModule");

            var type = mm.DefineType("JSCodeClass",
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);

            type.DefineDefaultConstructor(MethodAttributes.Public);

            var method = type.DefineMethod("Run", MethodAttributes.Public | MethodAttributes.Static,
                exp.ReturnType,
                exp.Parameters.Select(p => p.Type).ToArray());

            // Expression<Func<string,string>> y = x => this.Simple<string>(() => x == null ? x : null);

            ExpressionCompiler.CompileToMethod(exp, method);

            var t = type.CreateTypeInfo();
            var m = t.GetMethod("Run");

            return (T)(object)m.CreateDelegate(typeof(T));
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

        internal static (MethodInfo method,string il, string exp) Compile(YLambdaExpression expression,
            IMethodRepository methods)
        {
            var exp = LambdaRewriter.Rewrite(expression)
                as YLambdaExpression;

            var runtimeMethodBuilder = new RuntimeMethodBuilder(methods);

            return InternalCompileToMethod(exp, runtimeMethodBuilder);
        }

        public static T Compile<T>(this YLambdaExpression expression)
        {
            var (m,_,_) = Compile(expression, null);
            return (T)(object)m.CreateDelegate(typeof(T));
        }

        public static T CompileWithNestedLambdas<T>(this YLambdaExpression expression)
        {
            var repository = new MethodRepository();

            var outerLambda = YExpression.Lambda(expression.Name + "_outer", expression, new List<YParameterExpression> { 
                YExpression.Parameter(typeof(IMethodRepository))
            });

            var (outer, il, exp) = Compile(outerLambda, repository);

            var func = outer.CreateDelegate(typeof(Func<IMethodRepository, object>)) as Func<IMethodRepository, object>;

            return (T)(object)func(repository);
        }


        internal static (MethodInfo method,string il, string exp) InternalCompileToMethod(
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

            return (dm, value, exp.DebugView);
        }

    }
}
