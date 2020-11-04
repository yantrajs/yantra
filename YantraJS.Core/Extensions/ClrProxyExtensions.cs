using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core;

namespace YantraJS
{
    public static class ClrProxyExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object CreateClrEnumerator(JSValue target, Type elementType) {
            Type type = typeof(ClrObjectEnumerator<>).MakeGenericType(elementType);
            return Activator.CreateInstance(type, target);
        }

        public static bool TryGetClrEnumerator(this JSValue value, Type type, out object clrObject)
        {
            if (type.IsConstructedGenericType)
            {
                var gt = type.GetGenericTypeDefinition();
                if (gt == typeof(IEnumerator<>))
                {
                    clrObject = CreateClrEnumerator(value, type.GetGenericArguments()[0]);
                    return true;
                }
            }

            if (type == typeof(System.Collections.IEnumerable))
            {
                clrObject = CreateClrEnumerator(value, typeof(object));
                return true;
            }
            clrObject = null;
            return false;
        }

    }
}
