﻿using System;
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

        public static T Compile<T>(this YExpression<T> exp)
        {
            var originalTypes = exp.ParameterTypes;
            string expString = exp.ToString();
            exp = exp.PrefixParameter(typeof(Closures)).As<T>();

            var method = new DynamicMethod(exp.Name, exp.ReturnType, exp.ParameterTypes, typeof(Closures), true);

            var ilg = method.GetILGenerator();

            ILCodeGenerator icg = new ILCodeGenerator(ilg);
            icg.Emit(exp);

            string il = icg.ToString();

            var c = new Closures(null, il, expString);

            return (T)(object)method.CreateDelegate(typeof(T), c);
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

        public static T CompileWithNestedLambdas<T>(this YExpression<T> expression)
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

        private static ModuleBuilder moduleBuilder;

        static RuntimeAssembly()
        {
            var a = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("EC.Runtime"), AssemblyBuilderAccess.RunAndCollect);
            moduleBuilder = a.DefineDynamicModule("EC");
        }

        internal static Type CreateDelegateType(Type[] types, Type returnType)
        {
            return moduleBuilder.CreateDelegateType(types, returnType);
        }

        internal static TypeBuilder DefineDelegateType(this ModuleBuilder module, string name)
        {
            return module.DefineType(
                name,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass,
                typeof(MulticastDelegate)
            );
        }

        private static int did = 1;

        internal static Type CreateDelegateType(this ModuleBuilder module, Type[] types, Type returnType)
        {
            MethodAttributes CtorAttributes = MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;
            MethodImplAttributes ImplAttributes = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
            MethodAttributes InvokeAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
            Type[] s_delegateCtorSignature = { typeof(object), typeof(IntPtr) };

            TypeBuilder builder = module.DefineDelegateType("Delegate_" + Interlocked.Increment(ref did));
            builder.DefineConstructor(CtorAttributes, CallingConventions.Standard, s_delegateCtorSignature).SetImplementationFlags(ImplAttributes);
            builder.DefineMethod("Invoke", InvokeAttributes, returnType, types).SetImplementationFlags(ImplAttributes);
            return builder.CreateTypeInfo();
        }

    }
}