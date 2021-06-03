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
            var fx = exp.CompileInAssembly();
            // var fx = exp.Compile();
            return fx;
            // return (T)(object)((Delegate)(object)fx).ToTailDelegate(typeof(T), exp.Name ?? "tail_call");

        }

    }
}
