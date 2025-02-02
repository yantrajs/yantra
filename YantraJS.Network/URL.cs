using System;
using System.Collections.Generic;
using System.Text;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    [JSClassGenerator("URL")]
    public partial class URL: JSObject
    {
    
        public URL(in Arguments a): base(JSContext.NewTargetPrototype)
        {
            
        }
    }

    [JSClassGenerator]
    public partial class URLSearchParams: KeyValueStore
    {
        public URLSearchParams(in Arguments a): base(JSContext.NewTargetPrototype)
        {
            
        }
    }
}
