#nullable enable
using System;
using Yantra.Core;
using Yantra.Core.Events;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    [JSClassGenerator]
    public partial class AbortSignal: EventTarget
    {
        internal AbortSignal(in Arguments a): this(JSContext.NewTargetPrototype) {
        }


        [JSExport]
        public bool Aborted { get; internal set; }

        [JSExport]
        public string? Reason { get; private set; }

        public event EventHandler? AbortedEvent;

        internal void Abort(string? reason)
        {
            this.Reason = reason ?? "Aborted";
            Aborted = true;
            AbortedEvent?.Invoke(this, EventArgs.Empty);
            var e = Event.Create("abort");
            this.DispatchEvent(e);
        }
    }
}
