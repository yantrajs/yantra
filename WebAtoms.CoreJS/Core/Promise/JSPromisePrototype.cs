using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core.Runtime
{

    public static class JSPromisePrototype
    {

        private static JSPromise ToPromise(this JSValue t)
        {
            if (!(t is JSPromise p))
                throw JSContext.Current.NewTypeError($"Target isn't a promise");
            return p;
        }

        [Prototype("then")]
        public static JSValue Then(JSValue t, JSValue[] a)
        {
            var p = t.ToPromise();
            var f = a.GetAt(0);
            if (!(f is JSFunctionStatic fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            p.Then(fx.f);
            return t;
        }

        [Prototype("catch")]
        public static JSValue Catch(JSValue t, JSValue[] a)
        {
            var p = t.ToPromise();
            var f = a.GetAt(0);
            if (!(f is JSFunctionStatic fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            p.Catch(fx.f);
            return t;
        }

        [Prototype("finally")]
        public static JSValue Finally(JSValue t, JSValue[] a)
        {
            var p = t.ToPromise();
            var f = a.GetAt(0);
            if (!(f is JSFunctionStatic fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            p.Then(fx.f);
            p.Catch(fx.f);
            return t;
        }
    }
}
