using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Extensions
{
    public static class JSValueArrayExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue GetAt(this JSValue[] target, int index, JSValue def = null)
        {
            return target.Length > index ? target[index] : (def ?? JSUndefined.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAt(this JSValue[] target, int index, out JSValue value)
        {
            if (target.Length > index)
            {
                value = target[index];
                return true;
            }
            value = JSUndefined.Value;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (JSValue, JSValue[]) Slice(this JSValue[] target)
        {
            switch(target.Length)
            {
                case 0:
                    return (JSUndefined.Value, JSArguments.Empty);
                case 1:
                    return (target[0], JSArguments.Empty);
            }
            var copy = new JSValue[target.Length - 1] ;
            Array.Copy(target, 1, copy, 0, copy.Length);
            return (target[0], copy);
        }

    }
}
