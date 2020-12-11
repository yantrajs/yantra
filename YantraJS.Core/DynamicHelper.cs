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

            // may be this way we can move out from
            // here and move to different assembly...
            //var d = (Delegate)(object)fx;
            //var type = typeof(System.Reflection.Emit.DynamicMethod);
            //var rtd = type.GetNestedType("RTDynamicMethod", System.Reflection.BindingFlags.NonPublic);
            //var field = rtd.GetTypeInfo().DeclaredFields.FirstOrDefault(x => x.Name == "m_owner");
            //DynamicMethod dm = field?.GetValue(d.Method) as DynamicMethod;
            //var il = dm.GetILGenerator();
            return fx;
        }

    }
}
