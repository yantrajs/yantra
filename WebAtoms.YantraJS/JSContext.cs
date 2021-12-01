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

        public ClrClassFactory ClassFactory { get; set; } = ClrClassFactory.Default;

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
            return new AtomEnumerable(new JSArray(ArrayPrototype));
        }

        public IJSValue CreateDate(DateTime value)
        {
            return new JSDate(DatePrototype, value);
        }

        public IJSValue CreateConstructor(int numberOfParameters, Func<IJSContext, IList<IJSValue>, IJSValue> func, string debugDescription)
        {
            return new JSFunction(FunctionPrototype, (in Arguments a) =>
            {
                return func(this, a.ToList()).ToJSValue();
            }, debugDescription, StringSpan.Empty, numberOfParameters);
        }

        public IJSValue CreateFunction(int numberOfParameters, Func<IJSContext, IList<IJSValue>, IJSValue> func, string debugDescription)
        {
            return new JSFunction(FunctionPrototype, (in Arguments a) =>
            {
                return func(this, a.ToList()).ToJSValue();
            }, debugDescription, StringSpan.Empty, numberOfParameters);
        }

        public IJSValue CreateNumber(double number)
        {
            return new JSNumber(number);
        }

        public IJSValue CreateObject()
        {
            return new JSObject(ObjectPrototype);
        }

        public IJSValue CreateString(string text)
        {
            return new JSString(StringPrototype, text);
        }

        public IJSValue CreateSymbol(string name)
        {
            return new JSSymbol(name);
        }

        public IJSValue Evaluate(string script, string location = null)
        {
            return FastEval(script, location);
        }

        public Task EvaluateAsync(string script, string location = null)
        {
            return Task.Run(() => FastEval(script, location));
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
            if (value == this)
                return this;
            var proxy = new ClrProxy(value);
            return proxy;
        }

        public IJSValue CreateBoundFunction(int numberOfParameters, WJSBoundFunction func, string debugDescription)
        {
            return new JSFunction(FunctionPrototype, (in Arguments a) =>
            {
                return func(this, a.This, a.ToList()).ToJSValue();
            }, debugDescription, StringSpan.Empty, numberOfParameters);
        }

    }
}
