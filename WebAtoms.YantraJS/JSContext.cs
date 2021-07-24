using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAtoms;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Core
{

    public partial class JSContext : IJSContext
    {

        (JSFunction appendChildFx,
            JSFunction dispatchEventFx,
            JSFunction addEventListenerFx,
            KeyString appendChild,
            KeyString dispatchEvent,
            KeyString addEventListener,
            JSValue bridge) elementWrapper;

        IJSValue IJSContext.this[string name]
        {
            get => this[name];
            set => this[name] = value.ToJSValue();
        }
        IJSValue IJSContext.this[IJSValue keyOrSymbol]
        {
            get =>
                this[keyOrSymbol as JSValue];
            set =>
                this[keyOrSymbol as JSValue] = value.ToJSValue();
        }

        //public IJSValue this[string name] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public IJSValue this[IJSValue keyOrSymbol] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IJSValue Undefined => JSUndefined.Value;

        public IJSValue Null => JSNull.Value;

        public IJSValue True => JSBoolean.True;

        public IJSValue False => JSBoolean.False;

        public string Stack => new JSException("").JSStackTrace.ToString();

        public event EventHandler<ErrorEventArgs> ErrorEvent;

        partial void OnError(Exception ex)
        {
            ErrorEvent?.Invoke(this, new ErrorEventArgs()
            {
                Error = ex.Message,
                Stack = ex.StackTrace
            });
        }

        public IJSArray CreateArray()
        {
            return new AtomEnumerable(new JSArray());
        }

        public IJSValue CreateDate(DateTime value)
        {
            return new JSDate(value);
        }

        public IJSValue CreateFunction(int numberOfParameters, Func<IJSContext, IList<IJSValue>, IJSValue> func, string debugDescription)
        {
            return new JSFunction((in Arguments a) =>
            {
                return func(this, a.ToList()).ToJSValue();
            }, debugDescription, numberOfParameters);
        }

        public IJSValue CreateNumber(double number)
        {
            return new JSNumber(number);
        }

        public IJSValue CreateObject()
        {
            return new JSObject();
        }

        public IJSValue CreateString(string text)
        {
            return new JSString(text);
        }

        public IJSValue CreateSymbol(string name)
        {
            return new JSSymbol(name);
        }

        public IJSValue Evaluate(string script, string location = null)
        {
            return CoreScript.Evaluate(script, location);
        }

        public void RunOnUIThread(Func<Task> task)
        {
            var current = this;
            synchronizationContext.Post(async (a) =>
            {
                JSContext.Current = current;
                _current.Value = current;
                Func<Task> t = a as Func<Task>;
                try
                {
                    await t();
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                }
            }, task);
        }

        public IJSValue Wrap(object value)
        {
            var proxy = new ClrProxy(value);
            if (elementWrapper.appendChildFx == null)
            {
                elementWrapper.bridge = this["bridge"];
                elementWrapper.appendChild = "appendChild";
                elementWrapper.dispatchEvent = "dispatchEvent";
                elementWrapper.addEventListener = "addEventListener";
                elementWrapper.appendChildFx = new JSFunction(AppendChild);
                elementWrapper.dispatchEventFx = new JSFunction(DispatchEvent);
                elementWrapper.addEventListenerFx = new JSFunction(AddEventListener);
            }
            proxy[elementWrapper.appendChild] = elementWrapper.appendChildFx;
            proxy[elementWrapper.dispatchEvent] = elementWrapper.dispatchEventFx;
            proxy[elementWrapper.addEventListener] = elementWrapper.addEventListenerFx;
            return proxy;
        }

        private JSValue AppendChild(in Arguments a)
        {
            return elementWrapper.bridge.InvokeMethod(elementWrapper.appendChild,
                new Arguments(elementWrapper.bridge, a.This, a.Get1()));
        }

        private JSValue DispatchEvent(in Arguments a)
        {
            return elementWrapper.bridge.InvokeMethod(elementWrapper.dispatchEvent,
                new Arguments(elementWrapper.bridge, a.This, a.Get1()));
        }

        private JSValue AddEventListener(in Arguments a)
        {
            return elementWrapper.bridge.InvokeMethod(elementWrapper.addEventListener,
                new Arguments(elementWrapper.bridge, a.This, a.Get1()));
        }

        public IJSValue CreateBoundFunction(int numberOfParameters, WJSBoundFunction func, string debugDescription)
        {
            return new JSFunction((in Arguments a) =>
            {
                return func(this, a.This, a.ToList()).ToJSValue();
            }, debugDescription, numberOfParameters);
        }

    }
}

//namespace YantraJS.Core.Clr { 
//    public partial class ClrProxy
//    {

//    }
//}

