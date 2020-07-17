using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSFunction : JSObject
    {

        private JSValue bound;

        internal JSFunctionDelegate f;
        internal JSFunction(JSFunctionDelegate f)
        {
            this.f = f;
        }

        public override JSValue CreateInstance(JSArray args)
        {
            var cx = JSContext.Current;
            JSValue obj = cx.CreateObject();
            obj.prototype = this;
            obj = f(obj, args);
            return obj;
        }

        public override JSValue InvokeFunction(JSValue thisValue, JSArray args)
        {
            return f(bound ?? thisValue, args);
        }

        internal static JSProperty call = JSProperty.Function((t, a) => {
            JSArray ar = a as JSArray;
            return t.InvokeFunction(ar[0], ar.Slice(1));
        });

        internal static JSProperty apply = JSProperty.Function((t, a) => {
            JSArray ar = a as JSArray;
            return t.InvokeFunction(ar[0], ar[1] as JSArray);
        });

        internal static JSProperty bind = JSProperty.Function((t, a) => {
            var fOriginal = (JSFunction)t;
            var tx = a[0];
            var fx = new JSFunction((bt, ba) => fOriginal.f(tx, ba));
            return fx;
        });
    }
}
