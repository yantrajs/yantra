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
        public static JSValue Then(in Arguments a)
        {
            var p = a.This.ToPromise();
            var f = a.Get1();
            if (!(f is JSFunction fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            p.Then(fx.f);
            return p;
        }

        [Prototype("catch")]
        public static JSValue Catch(in Arguments a)
        {
            var p = a.This.ToPromise();
            var f = a.Get1();
            if (!(f is JSFunction fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            p.Catch(fx.f);
            return p;
        }

        [Prototype("finally")]
        public static JSValue Finally(in Arguments a)
        {
            var p = a.This.ToPromise();
            var f = a.Get1();
            if (!(f is JSFunction fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            p.Then(fx.f);
            p.Catch(fx.f);
            return p;
        }
    }
}
