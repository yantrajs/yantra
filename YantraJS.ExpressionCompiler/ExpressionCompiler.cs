using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using YantraJS.Builder;
using YantraJS.Core;
using YantraJS.Expressions;
using YantraJS.Generator;

namespace YantraJS
{
    public interface IMethodRepository
    {
        object Create(Box[] boxes, ulong id);
        ulong RegisterNew(DynamicMethod d, string il, string exp, Type type);
    }

    public static class ExpressionCompiler
    {

        private static int id = 1;

        internal static string GetUniqueName(in FunctionName name)
        {
            return $"<YantraJSHidden>{name}<ID>{System.Threading.Interlocked.Increment(ref id)}";
        }

        private static ConstructorInfo locationConstructor = typeof(LocationAttribute).GetConstructor(new Type[] {
            typeof(string),
            typeof(string),
            typeof(int),
            typeof(int)
        });

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
                LambdaRewriter.Rewrite(lambdaExpression);
            }

            //var method = type.DefineMethod(GetUniqueName(lambdaExpression.Name),
            //    MethodAttributes.Public, CallingConventions.HasThis,
            //    lambdaExpression.ReturnType,
            //    lambdaExpression.ParameterTypes);

            var method = type.CreateMethod(lambdaExpression, GetUniqueName(lambdaExpression.Name), true);

            var ln = lambdaExpression.Name;
            if (ln.Location != null)
            {
                var cb = new CustomAttributeBuilder(locationConstructor, new object[] { ln.Location, ln.Name, ln.Line, ln.Column });
                method.SetCustomAttribute(cb);
            }

            //NestedRewriter nw = new NestedRewriter(lambdaExpression, new LambdaMethodBuilder(method));
            //lambdaExpression = nw.Visit(lambdaExpression) as YLambdaExpression;

            var ilw = new StringWriter();
            var ew = new StringWriter();

            ILCodeGenerator icg = new ILCodeGenerator(method.GetILGenerator(), new LambdaMethodBuilder(method), ilw, ew);
            icg.Emit(lambdaExpression);

            return (method, ilw.ToString(), ew.ToString());
        }


        ///// <summary>
        ///// For debug = true, save it as an instance method..
        ///// and in static method create an instance of Closure
        ///// and return the instance method
        ///// </summary>
        ///// <param name="lambdaExpression"></param>
        ///// <param name="type"></param>
        ///// <param name="debug"></param>
        ///// <returns></returns>
        //public static MethodInfo CompileToStaticMethod(
        //    this YLambdaExpression lambdaExpression,
        //    TypeBuilder type,
        //    PdbBuilder builder
        //    )
        //{
        //    var exp = LambdaRewriter.Rewrite(lambdaExpression)
        //        as YLambdaExpression;

        //    var method = type.DefineMethod(GetUniqueName(lambdaExpression.Name),
        //        MethodAttributes.Public | MethodAttributes.Static,
        //        exp.ReturnType,
        //        exp.ParameterTypes);

        //    NestedRewriter nw = new NestedRewriter(exp, new LambdaMethodBuilder(method));
        //    exp = nw.Visit(exp) as YLambdaExpression;

        //    var mdh = MetadataTokens.MethodDefinitionHandle(builder.Next);


        //    ILCodeGenerator icg = new ILCodeGenerator(method.GetILGenerator());
        //    var (ict, iexp) = icg.Emit(exp);

        //    return method;
        //}

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
                var m = type.DefineMethod(lambdaExpression.Name + "_Inner_Factory",
                    MethodAttributes.Public | MethodAttributes.Static, typeof(object), new Type[] { });

                return CompileToStaticMethod(lambdaExpression, type, m, debug);
            }

            //var exp = LambdaRewriter.Rewrite(lambdaExpression)
            //    as YLambdaExpression;

            var method = type.DefineMethod(GetUniqueName(lambdaExpression.Name), 
                MethodAttributes.Public | MethodAttributes.Static,
                lambdaExpression.ReturnType,
                lambdaExpression.ParameterTypes);

            var ln = lambdaExpression.Name;
            if(ln.Location != null)
            {
                var cb = new CustomAttributeBuilder(locationConstructor, new object[] { ln.Location, ln.Name, ln.Line, ln.Column });
                method.SetCustomAttribute(cb);
            }

            //NestedRewriter nw = new NestedRewriter(exp, new LambdaMethodBuilder(method));
            //exp = nw.Visit(exp) as YLambdaExpression;

            ILCodeGenerator icg = new ILCodeGenerator(method.GetILGenerator(), new LambdaMethodBuilder(method));
            icg.Emit(lambdaExpression);

            return method;
        }

        public static MethodInfo CompileToStaticMethod(
            this YLambdaExpression lambdaExpression, 
            TypeBuilder type,
            MethodBuilder method, bool debug = false)
        {

            var derived = (type.Module as ModuleBuilder).DefineType(
                ExpressionCompiler.GetUniqueName(lambdaExpression.Name.FullName ?? "Closures"),
                TypeAttributes.Public,
                typeof(Closures));

            var (im, il, exp) = lambdaExpression
                .WithThis(typeof(Closures))
                .CompileToInstnaceMethod(derived);

            exp = null;
            il = null;

            var cnstr = derived.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] {
                typeof(Box[])
            });

            var boxes = YExpression.Parameter(typeof(Box[]));

            var cnstrLambda = YExpression.Lambda(lambdaExpression.Type, "cnstr",
                YExpression.CallNew(Closures.constructor,
                    YExpression.New(MethodRepository.constructor),
                    boxes,
                    YExpression.Constant(il),
                    YExpression.Constant(exp)),
                new YParameterExpression[] { YExpression.Parameter(derived), boxes });

            var cnstrIL = new ILCodeGenerator(cnstr.GetILGenerator(), null);
            cnstrIL.EmitConstructor(cnstrLambda);

            // string cnstrILText = cnstrIL.ToString();

            var dt = lambdaExpression.Type;

            var cdt = dt.GetConstructors().First(x => x.GetParameters().Length == 2);

            var cd = typeof(MethodInfo).GetMethod(nameof(MethodInfo.CreateDelegate), new Type[] {
                typeof(Type),
                method.ReturnType 
            });

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

            //var m = type.DefineMethod(lambdaExpression.Name + "_Inner_Factory",
            //    MethodAttributes.Public | MethodAttributes.Static, typeof(object), new Type[] { });
            var m = method;

            var ln = lambdaExpression.Name;
            if (ln.Location != null)
            {
                var cb = new CustomAttributeBuilder(locationConstructor, new object[] { ln.Location, ln.Name, ln.Line, ln.Column });
                m.SetCustomAttribute(cb);
            }

            var icg = new ILCodeGenerator(m.GetILGenerator(), null);

            icg.Emit(create);

            return m;
        }
    }
}
