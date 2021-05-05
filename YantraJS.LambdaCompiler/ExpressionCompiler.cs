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
        object Create(Box[] boxes, int id);
        int RegisterNew(DynamicMethod d, string il, string exp, Type type);
    }

    public static class ExpressionCompiler
    {

        private static int id = 1;

        internal static string GetUniqueName(string name)
        {
            return $"<YantraJSHidden>{name}<ID>{System.Threading.Interlocked.Increment(ref id)}";
        }

        public static T CompileInAssembly<T>(this YExpression<T> exp)
        {
            AssemblyName name = new AssemblyName("demo");

            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);

            var mm = ab.DefineDynamicModule("JSModule");

            var type = mm.DefineType("JSCodeClass",
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);

            var method = exp.CompileToStaticMethod(type, true);
            var t = type.CreateTypeInfo();
            var m = t.GetMethods().FirstOrDefault(x => x.Name == method.Name);
            return (T)m.Invoke(null, new object[] { });
        }


        public static (MethodInfo method, string il, string exp) CompileToInstnaceMethod(
            this YLambdaExpression lambdaExpression,
            TypeBuilder type, bool rewriteNestedLambda = true
            )
        {
            if (!lambdaExpression.This.Type.IsAssignableFrom(type))
                throw new NotSupportedException($"First parameter of an instance method must be same as the owner type");

            if (rewriteNestedLambda)
            {
                lambdaExpression = LambdaRewriter.Rewrite(lambdaExpression)
                    as YLambdaExpression;
            }

            string exp = lambdaExpression.ToString();

            var method = type.DefineMethod(GetUniqueName(lambdaExpression.Name),
                MethodAttributes.Public, CallingConventions.HasThis,
                lambdaExpression.ReturnType,
                lambdaExpression.ParameterTypes);

            NestedRewriter nw = new NestedRewriter(lambdaExpression, new LambdaMethodBuilder(method));
            lambdaExpression = nw.Visit(lambdaExpression) as YLambdaExpression;

            ILCodeGenerator icg = new ILCodeGenerator(method.GetILGenerator());
            icg.Emit(lambdaExpression);

            return (method, icg.ToString(), exp);
        }


        /// <summary>
        /// For debug = true, save it as an instance method..
        /// and in static method create an instance of Closure
        /// and return the instance method
        /// </summary>
        /// <param name="lambdaExpression"></param>
        /// <param name="type"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static MethodInfo CompileToStaticMethod(
            this YLambdaExpression lambdaExpression,
            TypeBuilder type, bool debug = false)
        {
            if(debug)
            {
                return CompileToInternalMethod(lambdaExpression, type);
            }

            var exp = LambdaRewriter.Rewrite(lambdaExpression)
                as YLambdaExpression;

            var method = type.DefineMethod(GetUniqueName(lambdaExpression.Name), 
                MethodAttributes.Public | MethodAttributes.Static,
                exp.ReturnType,
                exp.ParameterTypes);

            NestedRewriter nw = new NestedRewriter(exp, new LambdaMethodBuilder(method));
            exp = nw.Visit(exp) as YLambdaExpression;

            ILCodeGenerator icg = new ILCodeGenerator(method.GetILGenerator());
            icg.Emit(exp);

            return method;
        }

        internal static MethodInfo CompileToInternalMethod(
            this YLambdaExpression lambdaExpression, 
            TypeBuilder type)
        {

            var derived = (type.Module as ModuleBuilder).DefineType(
                ExpressionCompiler.GetUniqueName("Closures"),
                TypeAttributes.Public,
                typeof(Closures));

            var (im, il, exp) = lambdaExpression
                .WithThis(typeof(Closures))
                .CompileToInstnaceMethod(derived);

            var cnstr = derived.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] {
                typeof(Box[])
            });

            var boxes = YExpression.Parameter(typeof(Box[]));

            var cnstrLambda = YExpression.Lambda(lambdaExpression.Type, "cnstr",
                YExpression.CallNew(Closures.constructor, boxes, YExpression.Constant(il), YExpression.Constant(exp)),
                new YParameterExpression[] { YExpression.Parameter(derived), boxes });

            var cnstrIL = new ILCodeGenerator(cnstr.GetILGenerator());
            cnstrIL.EmitConstructor(cnstrLambda);

            string cnstrILText = cnstrIL.ToString();

            var dt = lambdaExpression.Type;

            var cdt = dt.GetConstructors().First(x => x.GetParameters().Length == 2);

            var cd = typeof(MethodInfo).GetMethod(nameof(MethodInfo.CreateDelegate), new Type[] { typeof(Type), typeof(object) });

            var derivedType = derived.CreateTypeInfo();
            var ct = derivedType.GetConstructors()[0];

            //var create = YExpression.Lambda( "Create", YExpression.Call(YExpression.Constant(im), cd, 
            //    YExpression.Constant(dt), 
            //    YExpression.New(cnstr, YExpression.Null)
            //    ), new YParameterExpression[] { });

            var create = YExpression.Lambda( lambdaExpression.Type, "Create", 
                
                YExpression.New(cdt,
                    YExpression.New(cnstr, YExpression.Null),
                    YExpression.Constant(im))
                , new YParameterExpression[] { });

            var m = type.DefineMethod(lambdaExpression.Name + "_Inner_Factory",
                MethodAttributes.Public | MethodAttributes.Static, typeof(object), new Type[] { });

            var icg = new ILCodeGenerator(m.GetILGenerator());

            icg.Emit(create);

            var il1 = icg.ToString();

            return m;
        }
    }
}
