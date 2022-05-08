using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Extensions;

namespace YantraJS.Core.Weak
{
    public class JSFinalizationRegistry: JSObject
    {
        private readonly JSSymbol finalizationSymbol = new JSSymbol("finalization");

        private readonly JSFunction finalizer;

        public JSFinalizationRegistry(JSFunction finalizer): base(JSContext.Current.FinalizationRegistryPrototype)
        {
            this.finalizer = finalizer;
        }

        public class WeakObject: JSObject
        {
            private readonly JSFinalizationRegistry registry;
            private readonly JSObject weakRef;

            public WeakObject(JSFinalizationRegistry registry, JSObject weakRef)
            {
                this.registry = registry;
                this.weakRef = weakRef;
            }

            ~WeakObject()
            {
                registry.FinalizeReference(weakRef);
            }
        }

        private void FinalizeReference(JSObject weakRef)
        {
            finalizer.InvokeFunction(new Arguments(this, weakRef));
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            if (!(a[0] is JSFunction fx))
                throw JSContext.Current.NewTypeError($"Argument is not a function");
            return new JSFinalizationRegistry(fx);
        }

        [Prototype("unregister")]
        public static JSValue Unregister(in Arguments a)
        {
            if (!(a.This is JSFinalizationRegistry @this))
                throw JSContext.Current.NewTypeError($"Invalid receiver");
            if (!(a[0] is JSObject obj))
                throw JSContext.Current.NewTypeError($"Argument is not an object");
            var weakRef = obj[@this.finalizationSymbol] as WeakObject;
            GC.SuppressFinalize(weakRef);
            return JSUndefined.Value;
        }

        [Prototype("register")]
        public static JSValue Register(in Arguments a)
        {
            if (!(a.This is JSFinalizationRegistry @this))
                throw JSContext.Current.NewTypeError($"Invalid receiver");
            if (!(a[0] is JSObject obj))
                throw JSContext.Current.NewTypeError($"Argument is not an object");
            obj[@this.finalizationSymbol] = new WeakObject(@this, obj);
            return JSUndefined.Value;
        }
    }

    public class JSWeakRef: JSObject
    {

        internal WeakReference<JSValue> weak;

        public JSWeakRef(JSValue value): base(JSContext.Current.WeakRefPrototype)
        {
            weak = new WeakReference<JSValue>(value);
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            return new JSWeakRef(a.Get1());
        }

        [Prototype("deref")]
        public static JSValue Deref(in Arguments a)
        {
            if (!(a.This is JSWeakRef wr))
                throw JSContext.Current.NewTypeError("WeakRef.prototype.deref receiver is not WeakRef");
            if (wr.weak.TryGetTarget(out var v))
                return v;
            return JSUndefined.Value;
        }

    }
}
