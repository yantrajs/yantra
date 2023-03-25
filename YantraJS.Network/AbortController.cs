#nullable enable
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    public class AbortController : JavaScriptObject
    {
        public AbortController(in Arguments a) : base(a)
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
