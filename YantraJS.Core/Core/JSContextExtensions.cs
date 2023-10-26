using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.Core
{
    public static class JSContextExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureSufficientExecutionStack(this JSContext context)
        {
#if NETSTANDARD2_1_OR_GREATER
            if(RuntimeHelpers.TryEnsureSufficientExecutionStack())
            {
                return;
            }
#else
            try
            {
                RuntimeHelpers.EnsureSufficientExecutionStack();
                return;
            }
            catch (Exception ex)
            {
                if (ex is not InsufficientExecutionStackException)
                {
                    throw ex;
                }
            }
#endif
            throw context.NewRangeError("Maximum call stack size exceeded");
        }


    }
}
