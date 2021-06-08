using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using YantraJS.Core;
using YantraJS.Expressions;
using YantraJS.Generator;

namespace YantraJS.Runtime
{
    public static class RuntimeAssembly
    {

        public static object Compile(this YLambdaExpression exp)
        {
            var originalTypes = exp.ParameterTypes;
            string expString = exp.ToString();
            exp = exp.WithThis(typeof(Closures));

            //var f = new FlattenVisitor();
            //exp = f.Visit(exp) as YLambdaExpression;

            var method = new DynamicMethod(exp.Name.FullName, exp.ReturnType, exp.ParameterTypesWithThis, typeof(Closures), true);
            
            var ilg = method.GetILGenerator();

            ILCodeGenerator icg = new ILCodeGenerator(ilg);
            icg.Emit(exp);

            string il = icg.ToString();

            var c = new Closures(null, il, expString);

            return (object)method.CreateDelegate(exp.Type, c);
        }

        public static T Compile<T>(this YExpression<T> exp)
        {
            var originalTypes = exp.ParameterTypes;
            string expString = exp.ToString();
            exp = exp.WithThis<T>(typeof(Closures));

            // var f = new FlattenVisitor();

            var method = new DynamicMethod(exp.Name.FullName, exp.ReturnType, exp.ParameterTypesWithThis, typeof(Closures), true);

            var ilg = method.GetILGenerator();

            var sw = new StringWriter();
            var expWriter = new StringWriter();
            ILCodeGenerator icg = new ILCodeGenerator(ilg, sw, expWriter);
            icg.Emit(exp);

            string il = sw.ToString();

            var c = new Closures(null, il, expWriter.ToString());
            try
            {
                return (T)(object)method.CreateDelegate(typeof(T), c);
            } catch (Exception ex)
            {
                throw;
            }
        }


        internal static (DynamicMethod, string il, string exp) CompileToBoundDynamicMethod(
            this YLambdaExpression exp, Type boundType = null)
        {
            // create closure...

            boundType = boundType ?? typeof(Closures);

            // dynamic method expects this as first parameter !!


            var method = new DynamicMethod(exp.Name.FullName, exp.ReturnType, exp.ParameterTypesWithThis, boundType, true);

            var ilg = method.GetILGenerator();
            StringWriter sw = new StringWriter();
            var expWriter = new StringWriter();
            ILCodeGenerator icg = new ILCodeGenerator(ilg, sw, expWriter);
            icg.Emit(exp);

            string il = sw.ToString();

            return (method, il, expWriter.ToString());

        }

        public static T CompileWithNestedLambdas<T>(this YExpression<T> expression)
        {
            var repository = new MethodRepository();

            var outerLambda = YExpression
                .InstanceLambda<Func<T>>(
                    expression.Name + "_outer", 
                    expression, 
                    YExpression.Parameter(typeof(MethodRepository))
                    , new YParameterExpression[] { })
                as YLambdaExpression;


            //var f = new FlattenVisitor();
            //outerLambda = f.Visit(outerLambda) as YLambdaExpression;

            outerLambda = LambdaRewriter.Rewrite(outerLambda)
                as YLambdaExpression;



            var runtimeMethodBuilder = new RuntimeMethodBuilder(repository);

            NestedRewriter nw = new NestedRewriter(outerLambda, runtimeMethodBuilder);

            outerLambda = nw.Visit(outerLambda) as YLambdaExpression;

            var (outer, il, exp) = outerLambda.CompileToBoundDynamicMethod(typeof(MethodRepository));

            repository.IL = il;
            repository.Exp = exp;

            // var fx = Delegate.CreateDelegate(typeof(Func<T>), repository, outer, true);
            var func = outer.CreateDelegate(typeof(Func<T>), repository) as Func<T>;

            return func();

            // return (T)(object)outer.CreateDelegate(typeof(Func<T>), repository);
            // return (T)func.DynamicInvoke();
        }

    }
}
