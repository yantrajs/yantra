using System;
using System.Collections.Generic;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core
{
    //internal class ModuleCache: ConcurrentSharedStringTrie<JSModule>
    //{
    //    internal static Key module = "module";
    //    internal static Key clr = "clr";
    //}

    public struct ModuleCache
    {
        private static ConcurrentNameMap nameCache;
        private ConcurrentUInt32Map<JSModule> modules;
        

        static ModuleCache()
        {
            nameCache = new ConcurrentNameMap();
            module = nameCache.Get("module");
            clr = nameCache.Get("clr");
        }

        public static (uint Key, StringSpan Name) module;
        public static (uint Key, StringSpan Name) clr;

        public static ModuleCache Create()
        {
            return new ModuleCache(true);
        }
        public bool TryGetValue(in StringSpan key, out JSModule obj)
        {
            if(nameCache.TryGetValue(key, out var i))
            {
                if (modules.TryGetValue(i.Key, out obj))
                    return true;
            }
            obj = null;
            return false;
        }
        public JSModule GetOrCreate(in StringSpan key, Func<JSModule> factory)
        {
            var k = nameCache.Get(key);
            return modules.GetOrCreate(k.Key, factory);
        }

        public void Add(in StringSpan key, JSModule module)
        {
            var k = nameCache.Get(key);
            modules[k.Key] = module;
        }

        public ModuleCache(bool v)
        {
            modules = ConcurrentUInt32Map<JSModule>.Create();
        }

        public JSModule this[in (uint Key, StringSpan name) key]
        {
            get {
                if (modules.TryGetValue(key.Key, out var m))
                    return m;
                return null;
            }
            set {
                modules[key.Key] = value;
            }
        }

        public IEnumerable<JSModule> All => modules.All;
    }
}
