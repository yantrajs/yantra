using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using YantraJS.Core;
using YantraJS.Core.Debugger;

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
            V8InspectorProtocol p = V8InspectorProtocol.CreateInverseProxy(inverseWebSocketUri);
            p.ConnectAsync().ConfigureAwait(true);
            a.Debugger = p;
            a[KeyStrings.global] = a;
            p.AddContext(a);
            return a;
        }
    }
}
