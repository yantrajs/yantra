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

    }
}
