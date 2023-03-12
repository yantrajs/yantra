#nullable enable
using YantraJS.Core;

namespace Yantra.Core.Events
{
    public readonly struct DomEventHandler
    {
        public readonly DomEventHandlerDelegate? Delegate;
        public readonly JSFunction? JSDelegate;

        public readonly bool Once;
        public readonly bool Deferred;

        public DomEventHandler(
            DomEventHandlerDelegate @delegate,
            bool once = false,
            bool deferred = false
            )
        {
            Delegate = @delegate;
            JSDelegate = null;
            this.Once = once;
            this.Deferred = deferred;
        }
        public DomEventHandler(
            JSFunction @delegate,
            bool once = false,
            bool deferred = false
            )
        {
            JSDelegate = @delegate;
            Delegate = null;
            this.Once = once;
            this.Deferred = deferred;
        }
    }
}
