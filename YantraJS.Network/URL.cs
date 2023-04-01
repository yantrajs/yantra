using System;
using System.Collections.Generic;
using System.Text;
using Yantra.Core;
using YantraJS.Core;

namespace YantraJS.Network
{
    [JSClassGenerator("URL")]
    public partial class URL: JSObject
    {

        public URL(in Arguments a): this()
        {
            
        }
    }

    [JSClassGenerator]
    public partial class URLSearchParams: KeyValueStore
    {
        public URLSearchParams(in Arguments a): this()
        {
            
        }
    }
}
