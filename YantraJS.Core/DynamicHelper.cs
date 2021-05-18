using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core
{
    internal static class DynamicHelper
    {

        public static Delegate ToTailDelegate(this Delegate fx, Type delegateType, string name = null)
        {
            //// this will only work without nested lambda
            //// nested lambda needs to be extracted and put into external
            //// array of closures or static methods

            //// may be this way we can move out from
            //// here and move to different assembly...
            var d = (Delegate)(object)fx;
            var type = typeof(System.Reflection.Emit.DynamicMethod);
            var rtd = type.GetNestedType("RTDynamicMethod", System.Reflection.BindingFlags.NonPublic);
            var field = rtd.GetTypeInfo().DeclaredFields.FirstOrDefault(x => x.Name == "m_owner");
            DynamicMethod dm = field?.GetValue(d.Method) as DynamicMethod;
            var il = dm.GetILGenerator();
            var plist = dm.GetParameters().Length - 1;
            var newMethod = new DynamicMethod(name ?? "tail_call",
                dm.ReturnType,
                dm.GetParameters().Skip(1).Select(
                    x => x.IsIn || x.IsOut
                        ? (x.ParameterType.IsByRef 
                            ? x.ParameterType : x.ParameterType.MakeByRefType())
                        : x.ParameterType).ToArray(), true);
            var newIL = newMethod.GetILGenerator();

            newIL.Emit(OpCodes.Ldnull);
            for (int i = 0; i < plist; i++)
            {
                switch (i) {
                    case 0:
                        newIL.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        newIL.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        newIL.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        newIL.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        newIL.Emit(OpCodes.Ldarg_S, (short)i);
                        break;
                }
            }
            newIL.Emit(OpCodes.Tailcall);
            newIL.Emit(OpCodes.Call, dm);
            newIL.Emit(OpCodes.Ret);
            var newDelegate = newMethod.CreateDelegate(delegateType);
            return newDelegate;

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CompileFast<T>(this Expression<T> exp)
        {
            // return Microsoft.Scripting.Generation.CompilerHelpers.Compile<T>(exp, true);
            var fx = exp.FastCompile();
            // var fx = exp.Compile();
            return fx;
            // return (T)(object)((Delegate)(object)fx).ToTailDelegate(typeof(T), exp.Name ?? "tail_call");

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CompileDynamic<T>(this Expression<T> exp)
        {
            // return Microsoft.Scripting.Generation.CompilerHelpers.Compile<T>(exp, true);
            // var fx = exp.CompileInAssembly();
            var fx = exp.Compile();
            return fx;
            // return (T)(object)((Delegate)(object)fx).ToTailDelegate(typeof(T), exp.Name ?? "tail_call");

        }

    }
}
