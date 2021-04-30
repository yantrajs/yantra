using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using YantraJS.Expressions;
using YantraJS.Generator;

namespace YantraJS.Runtime
{
    public static class RuntimeAssembly
    {

        public static T Compile<T>(this YLambdaExpression exp)
        {
            var originalTypes = exp.ParameterTypes;
            string expString = exp.ToString();
            var extra = new List<YParameterExpression>() { YExpression.Parameter(typeof(Closures)) };
            var delegateTypes = new List<Type>() { typeof(Closures) };
            foreach(var p in exp.Parameters)
            {
                delegateTypes.Add(p.Type);
                extra.Add(p);
            }
            
            exp = new YLambdaExpression(exp.Name, exp.Body, extra.ToArray(), exp.ReturnType);

            var method = new DynamicMethod(exp.Name, exp.ReturnType, delegateTypes.ToArray(), typeof(Closures), true);

            var allTypes = new List<Type>(originalTypes);
            allTypes.Add(exp.ReturnType);

            var dt = System.Linq.Expressions.Expression.GetDelegateType(allTypes.ToArray());

            var ilg = method.GetILGenerator();

            ILCodeGenerator icg = new ILCodeGenerator(ilg);
            icg.Emit(exp);

            string il = icg.ToString();

            var c = new Closures(null, il, expString);
            var t = typeof(T);

            if (t != dt)
                throw new InvalidOperationException();
            return (T)(object)method.CreateDelegate(dt, c);
        }


        internal static (DynamicMethod, string il, string exp) CompileToBoundDynamicMethod(
            this YLambdaExpression exp, Type boundType = null)
        {
            // create closure...

            boundType = boundType ?? typeof(Closures);

            var method = new DynamicMethod(exp.Name , exp.ReturnType, exp.ParameterTypes, boundType, true);

            var ilg = method.GetILGenerator();

            ILCodeGenerator icg = new ILCodeGenerator(ilg);
            icg.Emit(exp);

            string il = icg.ToString();

            string expString = exp.ToString();

            return (method, il, expString);

        }

        public static T CompileWithNestedLambdas<T>(this YLambdaExpression expression)
        {
            var repository = new MethodRepository();

            var outerLambda = YExpression.Lambda(expression.Name + "_outer", expression, new List<YParameterExpression> {
                YExpression.Parameter(typeof(IMethodRepository))
            });

            outerLambda = LambdaRewriter.Rewrite(outerLambda)
                as YLambdaExpression;



            var runtimeMethodBuilder = new RuntimeMethodBuilder(repository);

            NestedRewriter nw = new NestedRewriter(outerLambda, runtimeMethodBuilder);

            outerLambda = nw.Visit(outerLambda) as YLambdaExpression;

            var (outer, il, exp) = outerLambda.CompileToBoundDynamicMethod(typeof(MethodRepository));

            repository.IL = il;
            repository.Exp = exp;

            var func = outer.CreateDelegate(typeof(Func<object>), repository) as Func<object>;

            return (T)(object)func();
        }

    }
}
