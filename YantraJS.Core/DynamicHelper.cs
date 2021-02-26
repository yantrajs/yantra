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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CompileDynamic<T>(this Expression<T> exp)
        {
            // return Microsoft.Scripting.Generation.CompilerHelpers.Compile<T>(exp, true);
            var fx = exp.Compile();

            return fx;

            //// this will only work without nested lambda
            //// nested lambda needs to be extracted and put into external
            //// array of closures or static methods

            //// may be this way we can move out from
            //// here and move to different assembly...
            //var d = (Delegate)(object)fx;
            //var type = typeof(System.Reflection.Emit.DynamicMethod);
            //var rtd = type.GetNestedType("RTDynamicMethod", System.Reflection.BindingFlags.NonPublic);
            //var field = rtd.GetTypeInfo().DeclaredFields.FirstOrDefault(x => x.Name == "m_owner");
            //DynamicMethod dm = field?.GetValue(d.Method) as DynamicMethod;
            //var il = dm.GetILGenerator();

            //var newMethod = new DynamicMethod(exp.Name ?? "tail_call",
            //    dm.ReturnType,
            //    exp.Parameters.Select(x => x.IsByRef ? (x.Type.IsByRef ? x.Type : x.Type.MakeByRefType()) : x.Type).ToArray(), true);
            //var newIL = newMethod.GetILGenerator();

            //newIL.Emit(OpCodes.Ldnull);
            //newIL.Emit(OpCodes.Ldarg_0);
            //newIL.Emit(OpCodes.Tailcall);
            //newIL.Emit(OpCodes.Call, dm);
            //newIL.Emit(OpCodes.Ret);
            //var newDelegate = newMethod.CreateDelegate(typeof(T));
            //return (T)(object)newDelegate;
        }

    }
}
