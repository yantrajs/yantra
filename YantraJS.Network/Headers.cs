#nullable enable
using System;
using System.Collections.Generic;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    [JSClassGenerator("Headers")]
    public partial class Headers : KeyValueStore
    {
        public Headers(in Arguments a) : base(JSContext.NewTargetPrototype)
        {
        }

        internal Headers(JSValue? first) : this()
        {
        }
    }
}
