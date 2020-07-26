using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Extensions
{
    internal static class JSPropertyExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSValue GetValue(this JSValue target, JSProperty p)
        {
            return p.IsValue ? p.value : p.get.f(target, JSArguments.Empty);
        }

    }
}
