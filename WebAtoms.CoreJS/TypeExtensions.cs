using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebAtoms.CoreJS
{
    internal static class TypeExtensions
    {

        public static MethodInfo StaticMethod(this Type type, string name, params Type[] types)
        {
            var m = type.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }

        public static MethodInfo StaticMethod<T1>(this Type type, string name)
        {
            var types = new Type[] { typeof(T1) };
            var m = type.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }

        public static MethodInfo StaticMethod<T1,T2>(this Type type, string name)
        {
            var types = new Type[] { typeof(T1), typeof(T2) };
            var m = type.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }

        public static MethodInfo StaticMethod<T1, T2, T3>(this Type type, string name)
        {
            var types = new Type[] { typeof(T1), typeof(T2), typeof(T3) };
            var m = type.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }

        public static ConstructorInfo Constructor(this Type type, params Type[] types)
        {
            var c = type.GetConstructor(types);
            if (c == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Constructor {type.Name}({tl}) not found");
            }
            return c;
        }
    }
}
