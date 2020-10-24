using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Core
{
    public static class MarshalExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Unmarshal<T>(this JSValue value)
        {
            return (T)Unmarshal(value, typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Unmarshal(this JSValue value, Type type)
        {
            if (value.TryConvertTo(type, out var obj))
                return obj;
            var target = Activator.CreateInstance(type);
            foreach(var p in type.GetProperties())
            {
                if (!p.CanWrite)
                    continue;
                var v = value[p.Name];
                if (v.IsNullOrUndefined)
                    continue;
                p.SetValue(target, v.Unmarshal(p.PropertyType));
            }
            return target;
        }
    }
}
