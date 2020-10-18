using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS
{
    public static class ClrProxyExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object CreateClrEnumerator(JSValue target, Type elementType) {
            Type type = typeof(ClrObjectEnumerator<>).MakeGenericType(elementType);
            return Activator.CreateInstance(type, target);
        }

        public static object GetClrEnumerator(this JSValue value, Type type)
        {
            if (type.IsConstructedGenericType)
            {
                var gt = type.GetGenericTypeDefinition();
                if (gt == typeof(IEnumerator<>))
                {
                    return CreateClrEnumerator(value, type.GetGenericArguments()[0]);
                }
            }

            throw new NotSupportedException();
        }

    }
}
