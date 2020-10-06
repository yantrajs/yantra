using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WebAtoms.CoreJS
{
    internal static class TypeExtensions
    {

        public static MethodInfo InternalMethod(this Type type, string name, params Type[] types)
        {
            var m = type.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }


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

        public static MethodInfo MethodStartsWith(this Type type, string name, params Type[] args)
        {
            var ms = type.GetMethods()
                .Where(x => x.Name == name);
            foreach(var m in ms)
            {
                var pl = m.GetParameters();
                if (pl.Length <= args.Length)
                    continue;
                int i = 0;
                bool found = true;
                foreach(var t in args)
                {
                    if (pl[i++].ParameterType != t)
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return m;
            }
            var tl = string.Join(",", args.Select(x => x.Name));
            throw new MethodAccessException($"Method not found {name}");
        }
    }
}
