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
    internal class CachedTypes : ConcurrentSharedStringTrie<ClrType> { 

    }
}
