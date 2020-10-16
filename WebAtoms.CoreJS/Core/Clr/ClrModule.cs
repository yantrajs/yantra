using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WebAtoms.CoreJS.Core.Storage;

namespace WebAtoms.CoreJS.Core.Clr
{
    public class ClrModule : JSObject
    {
        public ClrModule()
        {
        }

        private static ConcurrentStringTrie<ClrType> cache = new ConcurrentStringTrie<ClrType>();

        [Static("getClass")]
        public static JSValue GetClass(in Arguments a)
        {
            var a1 = a.Get1();
            if (!a1.BooleanValue)
                throw JSContext.Current.NewTypeError("First parameter should be non empty string");
            var name = a1.ToString();
            return cache.GetOrCreate(name, () => new ClrType(Type.GetType(name)));
        }
    }
    internal class Methods : ConcurrentUInt32Trie<(MethodBase method, ParameterInfo[] parameters)[]> { }

    internal class Properties : ConcurrentUInt32Trie<(PropertyInfo property, ParameterInfo[] parameters)[]> { }

    internal static class MethodNamesExtensions
    {
        public static Methods
            ToTrie(this IEnumerable<IGrouping<string, MethodInfo>> list) {
            var names = new Methods();
            foreach(var kvp in list)
            {
                var key = kvp.Key.ToCamelCase();
                List<(MethodBase method, ParameterInfo[] paramters)> methods = new List<(MethodBase method, ParameterInfo[] paramters)>();
                foreach(var m in kvp)
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
            foreach(var p in parameters)
            {
                if (p.HasDefaultValue)
                    continue;
                n++;
            }
            return n;
        }

    }

    internal class CachedTypes : ConcurrentSharedStringTrie<ClrType> { 

    }
}
