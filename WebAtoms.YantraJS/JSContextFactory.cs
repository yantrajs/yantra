using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using YantraJS.Core;

namespace WebAtoms.XF
{
    public class JSContextFactory : IJSContextFactory
    {
        public IJSContext Create()
        {
            // DictionaryCodeCache.Current = AssemblyCodeCache.Instance;
            var a = new JSContext(SynchronizationContext.Current);
            a[KeyStrings.global] = a;
            return a;
        }

        public IJSContext Create(Uri inverseWebSocketUri)
        {
            // DictionaryCodeCache.Current = AssemblyCodeCache.Instance;
            var a = new JSContext(SynchronizationContext.Current);
            a[KeyStrings.global] = a;
            return a;
        }
    }
}
