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

        public override JSValue InvokeFunction(JSValue thisValue, JSValue args)
        {
            return f(bound ?? thisValue, args);
        }

        internal static JSProperty call = JSProperty.Function((t, a) => {
            JSArray ar = a as JSArray;
            return t.InvokeFunction(ar[0], ar.Slice(1));
        });

        internal static JSProperty apply = JSProperty.Function((t, a) => {
            JSArray ar = a as JSArray;
            return t.InvokeFunction(ar[0], ar[1]);
        });

    }
}
