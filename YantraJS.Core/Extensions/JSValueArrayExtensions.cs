using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Extensions
{
    public static class JSValueArrayExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue GetAt(this JSValue[] target, int index, JSValue def = null)
        {
            return target.Length > index ? target[index] : (def ?? JSUndefined.Value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Get1(this JSValue[] target)
        {
            return target.Length > 0 ? target[0] : JSUndefined.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (JSValue,JSValue) Get2(this JSValue[] target)
        {
            switch(target.Length)
            {
                case 0:
                    return (JSUndefined.Value, JSUndefined.Value);
                case 1:
                    return (target[0], JSUndefined.Value);
            }
            return (target[0], target[1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (JSValue, JSValue, JSValue) Get3(this JSValue[] target)
        {
            switch (target.Length)
            {
                case 0:
                    return (JSUndefined.Value, JSUndefined.Value, JSUndefined.Value);
                case 1:
                    return (target[0], JSUndefined.Value, JSUndefined.Value);
                case 2:
                    return (target[0], target[1], JSUndefined.Value);
            }
            return (target[0], target[1], target[2]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (JSValue, JSValue, JSValue, JSValue) Get4(this JSValue[] target)
        {
            switch (target.Length)
            {
                case 0:
                    return (JSUndefined.Value, JSUndefined.Value, JSUndefined.Value, JSUndefined.Value);
                case 1:
                    return (target[0], JSUndefined.Value, JSUndefined.Value, JSUndefined.Value);
                case 2:
                    return (target[0], target[1], JSUndefined.Value, JSUndefined.Value);
                case 3:
                    return (target[0], target[1], target[2], JSUndefined.Value);
            }
            return (target[0], target[1], target[2], target[3]);
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
