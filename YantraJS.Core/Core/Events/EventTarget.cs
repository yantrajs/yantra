#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;

namespace Yantra.Core.Events
{

    public delegate JSValue DomEventHandlerDelegate(Event e);

    [JSClassGenerator]
    public partial class EventTarget: JSObject
    {
        public EventTarget(in Arguments a): this(JSContext.NewTargetPrototype)
        {

        }

        private static ConcurrentNameMap eventNames = new ConcurrentNameMap();


        private SAUint32Map<List<DomEventHandler>> captureHandlers;
        private SAUint32Map<List<DomEventHandler>> handlers;

        [JSExport]
        public virtual JSValue DispatchEvent(Event e) {
            e.Target = this;
            e.CurrentTarget = this;
            e.EventPhase = Event.AT_TARGET;

            var pname = "on" + e.Type;
            var px = this[pname];
            if(px?.IsFunction ?? false)
            {
                px.Call();
            }

            // there is no capturing phase for simple events...
            KeyString name = e.Type;
            if(!handlers.TryGetValue(name.Key,out var list))
            {
                return e;
            }

            foreach(var handler in list.ToArray())
            {
                if (e.PropagationStopped)
                {
                    return e;
                }
                if (handler.JSDelegate != null)
                {
                    e.ReturnValue = handler.JSDelegate.InvokeFunction(new Arguments(this, e));
                    if (handler.Once) {
                        list.Remove(handler);
                    }
                    continue;
                }
                e.ReturnValue = handler.Delegate!.Invoke(e);
                if (handler.Once)
                {
                    list.Remove(handler);
                }
            }

            return e;
        }

        [JSExport]
        public JSValue AddEventListener(in Arguments a)
        {
            var name = a.GetString(0, "name");
            var listener = a[1] as JSFunction ?? throw new ArgumentException($"listener must be a function");
            var arg3 = a[2] ?? JSUndefined.Value;
            bool capture;
            var deferred = false;
            var once = false;
            if (arg3.IsObject)
            {
                capture = arg3[KeyStrings.capture].BooleanValue;
                deferred = arg3[KeyStrings.deferred].BooleanValue;
                once = arg3[KeyStrings.once].BooleanValue;
            } else
            {
                capture = arg3.BooleanValue;
            }
            AddEventHandler(name, new DomEventHandler(listener, once, deferred), capture);
            return JSUndefined.Value;
        }

        [JSExport]
        public JSValue RemoveEventListener(in Arguments a)
        {
            var name = a.GetString(0, "name");
            var listener = a[1] as JSFunction ?? throw new ArgumentException($"listener must be a function");
            var arg3 = a[2] ?? JSUndefined.Value;
            bool capture;
            var deferred = false;
            var once = false;
            if (arg3.IsObject)
            {
                capture = arg3[KeyStrings.capture].BooleanValue;
                deferred = arg3[KeyStrings.deferred].BooleanValue;
                once = arg3[KeyStrings.once].BooleanValue;
            }
            else
            {
                capture = arg3.BooleanValue;
            }
            RemoveEventHandler(name, new DomEventHandler(listener, once, deferred), capture);
            return JSUndefined.Value;
        }

        public IDisposable AddEventHandler(
            in StringSpan name, 
            in DomEventHandler handler, bool capture = false)
        {
            var (id,n) = eventNames.Get(in name);
            List<DomEventHandler> list;
            if (capture)
            {
                ref var node = ref captureHandlers.Put(id);
                node ??= new List<DomEventHandler>();
                list = node;
            } else
            {
                ref var node = ref handlers.Put(id);
                node ??= new List<DomEventHandler>();
                list = node;
            }
            list.Add(handler);
            return DisposableAction.Create((x) => list.Remove(x),handler);
        }

        public void RemoveEventHandler(in StringSpan name, in DomEventHandler @delegate, bool capture)
        {
            if (!eventNames.TryGetValue(name, out var id))
                return;
            List<DomEventHandler> list;
            if (capture)
            {
                if (!captureHandlers.TryGetValue(id.Key, out list))
                    return;
            } else
            {
                if (!handlers.TryGetValue(id.Key, out list))
                    return;
            }
            list.Remove(@delegate);
        }
    }
}
