#nullable enable
using System;
using System.Collections.Generic;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    public class Headers : KeyValueStore
    {
        public Headers(in Arguments a) : base(a)
        {
        }

        internal Headers(JSValue? first) : base(first)
        {
        }
    }
}
