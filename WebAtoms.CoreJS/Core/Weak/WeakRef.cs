using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core.Weak
{
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
