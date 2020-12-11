using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            return exp.Compile();
        }

    }
}
