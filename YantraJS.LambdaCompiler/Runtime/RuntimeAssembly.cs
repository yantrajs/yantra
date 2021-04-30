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

        private static ModuleBuilder moduleBuilder;

        private static int id = 1;

        static RuntimeAssembly()
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("EC.Runtime"),
                 AssemblyBuilderAccess.RunAndCollect);

            moduleBuilder = assemblyBuilder.DefineDynamicModule("YModule");
        }

        internal static (DynamicMethod, string il, string exp) CompileToBoundDynamicMethod(this YLambdaExpression exp)
        {
            // create closure...

            var method = new DynamicMethod(exp.Name , exp.ReturnType, exp.ParameterTypes, typeof(Closures), true);

            var ilg = method.GetILGenerator();

            ILCodeGenerator icg = new ILCodeGenerator(ilg);
            icg.Emit(exp);

            string il = icg.ToString();

            string expString = exp.ToString();

            return (method, il, expString);

        }


        internal static MethodInfo CompileToClosure(this YLambdaExpression exp)
        {
            // create closure...

            var type = moduleBuilder.DefineType($"YClosure_{Interlocked.Increment(ref id)}"
                , TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, typeof(Closures));

            // this is a Closure itself...
            var method = type.DefineMethod("Run", MethodAttributes.Public, CallingConventions.HasThis, exp.ReturnType, exp.ParameterTypes);

            var ilg = method.GetILGenerator();

            ILCodeGenerator icg = new ILCodeGenerator(ilg);
            icg.Emit(exp);

            string il = icg.ToString();

            string expString = exp.ToString();

            var defaultCC = typeof(Closures).GetConstructors()[0];

            var cc = type.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { typeof(Box[]) });

            ilg = cc.GetILGenerator();

            ilg.Emit(OpCodes.Ldarg_0); // load this
            ilg.Emit(OpCodes.Ldarg_1); // load boxes from first parameter...
            ilg.Emit(OpCodes.Ldstr, il); // load il 
            ilg.Emit(OpCodes.Ldstr, expString); // load il 
            ilg.Emit(OpCodes.Call, defaultCC);
            ilg.Emit(OpCodes.Ret);

            var t = type.CreateTypeInfo();
            return t.GetMethod("Run");

        }

    }
}
