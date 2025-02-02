using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Extensions;

namespace YantraJS.Core.Weak
{
    [JSClassGenerator("FinalizationRegistry")]
    public partial class JSFinalizationRegistry: JSObject
    {
        private readonly JSSymbol finalizationSymbol = new JSSymbol("finalization");

        private readonly JSSymbol finalizationToken = new JSSymbol("finalizationToken");

        private readonly JSFunction finalizer;

        public JSFinalizationRegistry(in Arguments a): base(JSContext.NewTargetPrototype)
        {
            if (a[0] is not JSFunction fx)
                throw JSContext.Current.NewTypeError($"Argument is not a function");
            this.finalizer = fx;
        }

        internal class WeakObject: JSObject
        {
            private readonly JSFinalizationRegistry registry;
            private readonly JSValue token;

            public WeakObject(JSFinalizationRegistry registry, JSValue token)
            {
                this.registry = registry;
                this.token = token;
            }

            ~WeakObject()
            {
                registry.FinalizeReference(token);
            }
        }

        private void FinalizeReference(JSValue token)
        {
            token.Delete(finalizationToken);
            finalizer.InvokeFunction(new Arguments(this, token));
        }

        [JSExport]
        public JSValue Unregister(in Arguments a)
        {
            if (!(a[0] is JSObject obj))
                throw JSContext.Current.NewTypeError($"Argument is not an object");
            // var weakRef = obj[@this.finalizationSymbol] as WeakObject;
            // GC.SuppressFinalize(weakRef);
            this.Unregister(a[0]);
            return JSUndefined.Value;
        }


        [JSExport]
        public JSValue Register(in Arguments a)
        {
            if (!(a[0] is JSObject obj))
                throw JSContext.Current.NewTypeError($"Argument is not an object");
            var token = a[1];
            if (token?.IsNullOrUndefined ?? false)
                throw JSContext.Current.NewTypeError($"Token is required");
            // obj[@this.finalizationSymbol] = new WeakObject(@this, a[1]);
            this.Register(obj, token);
            return JSUndefined.Value;
        }

        private void Register(JSValue target, JSValue token)
        {
            var weakRef = new WeakObject(this, token);
            target[finalizationSymbol] = weakRef;
            token[finalizationToken] = weakRef;
        }
        private void Unregister(JSValue token)
        {
            var weakRef = token[finalizationSymbol];
            token.Delete(finalizationSymbol);
            GC.SuppressFinalize(weakRef);
        }
    }

    [JSClassGenerator("WeakRef")]
    public  partial class JSWeakRef: JSObject
    {

        internal WeakReference<JSValue> weak;

        public JSWeakRef(JSValue value): this()
        {
            weak = new WeakReference<JSValue>(value);
        }

        public JSWeakRef(in Arguments a): base(JSContext.NewTargetPrototype)
        {
            weak = new WeakReference<JSValue>(a[0] ?? throw new JSException($"argument is missing"));
        }

        [JSExport]
        public JSValue Deref(in Arguments a)
        {
            if (weak.TryGetTarget(out var v))
                return v;
            return JSUndefined.Value;
        }

    }
}
