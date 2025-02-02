#nullable enable
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    [JSClassGenerator()]
    public partial class AbortController : JSObject
    {
        public AbortController(in Arguments a) : base(JSContext.NewTargetPrototype)
        {
            Signal = new AbortSignal();
        }

        public AbortSignal Signal { get; }

        [JSExport]
        public void Abort(string? name)
        {
            Signal.Abort(name);
        }
    }
}
