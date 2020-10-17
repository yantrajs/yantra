using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebAtoms.CoreJS.Core.Clr
{
    internal static class MethodNamesExtensions
    {
        public static (T method, ParameterInfo[] parameters)[] ToPairs<T>(this IEnumerable<T> methods)
            where T: MethodBase
        {
            return methods
                .Select(x => (x, x.GetParameters()))
                .ToArray();
        }

        public static Methods
            ToTrie(this IEnumerable<IGrouping<string, MethodInfo>> list)
        {
            var names = new Methods();
            foreach (var kvp in list)
            {
                var key = kvp.Key.ToCamelCase();
                List<(MethodBase method, ParameterInfo[] paramters)> methods = new List<(MethodBase method, ParameterInfo[] paramters)>();
                foreach (var m in kvp)
                {
                    methods.Add((m, m.GetParameters()));
                }
                names[KeyStrings.GetOrCreate(key).Key] = methods.OrderByDescending(x => x.paramters.RequiredCount()).ToArray();
            }
            return names;
        }

        public static Properties
            ToTrie(this IEnumerable<IGrouping<string, PropertyInfo>> list)
        {
            var names = new Properties();
            foreach (var kvp in list)
            {
                var key = kvp.Key.ToCamelCase();
                List<(PropertyInfo property, ParameterInfo[] parameters)> methods = new List<(PropertyInfo property, ParameterInfo[] parameters)>();
                foreach (var m in kvp)
                {
                    methods.Add((m, m.GetIndexParameters()));
                }
                names[KeyStrings.GetOrCreate(key).Key] = methods.OrderByDescending(x => x.parameters.RequiredCount()).ToArray();
            }
            return names;
        }

        public static int RequiredCount(this ParameterInfo[] parameters)
        {
            int n = 0;
            foreach (var p in parameters)
            {
                if (p.HasDefaultValue)
                    continue;
                n++;
            }
            return n;
        }

    }
}
