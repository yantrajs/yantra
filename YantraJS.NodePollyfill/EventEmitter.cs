using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Core.Core.Storage;

namespace YantraJS.NodePollyfill
{

    public class Event
    {

    }

    public class EventInfo
    {
        public JSValue Listener;

        public Action<Event> Delegate;

    }

    public class EventEmitter
    {

        private StringMap<List<JSValue>> listeners;

        private string ToKey(JSValue value)
            => value.IsString
                ? "key:" + value.ToString()
                : ((value is JSSymbol symbol)
                    ? "symbol:" + symbol.Key
                    : throw JSContext.CurrentContext.NewTypeError("Key can only be string or symbol"));

        public EventEmitter(in Arguments a)
        {

        }

        public JSValue On(in Arguments a)
        {
            var (eventName, listener) = a.Get2();
            this.AddEventListener(ToKey(eventName), listener);
            return JSUndefined.Value;
        }

        public JSValue Once(in Arguments a)
        {
            var (eventName, listener) = a.Get2();
            var key = ToKey(eventName);

            var task = new JSPromise((r, e) => {
                var remove = new JSFunction((in Arguments a1) => {
                    if (listeners.TryGetValue(key, out var list))
                    {
                        listener.InvokeFunction(in a1);
                        list.Remove(listener);
                        r(JSUndefined.Value);
                    }
                    return JSUndefined.Value;
                });

                AddEventListener(key, remove);
            });

            return task;
        }

        public JSValue Emit(in Arguments a)
        {
            var name = ToKey(a.Get1());

            if(listeners.TryGetValue(name,out var list))
            {

                var e = new Event();
                var jse = e.Marshal();
                foreach(var item in list)
                {
                    item.InvokeFunction(new Arguments(JSUndefined.Value, jse));
                }
            }

            return JSUndefined.Value;
        }

        private void AddEventListener(string name, JSValue handler)
        {
            if(!listeners.TryGetValue(name, out var list))
            {
                list = new List<JSValue>();
                listeners.Save(name, list);
            }

            list.Add(handler);
        }

        public JSValue AddListener(in Arguments a) => On(in a);

    }
}
