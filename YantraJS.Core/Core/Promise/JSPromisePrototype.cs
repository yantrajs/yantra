using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Extensions;

namespace YantraJS.Core.Runtime
{

    public static class JSPromisePrototype
    {

        private static JSPromise ToPromise(this JSValue t)
        {
            if (!(t is JSPromise p))
                throw JSContext.Current.NewTypeError($"Target isn't a promise");
            return p;
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            return new JSPromise(a.Get1());
        }

        [Prototype("then")]
        public static JSValue Then(in Arguments a)
        {
            var p = a.This.ToPromise();
            var f = a.Get1();
            if (!(f is JSFunction fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            return p.Then(fx.f, null);
        }

        [Prototype("catch")]
        public static JSValue Catch(in Arguments a)
        {
            var p = a.This.ToPromise();
            var f = a.Get1();
            if (!(f is JSFunction fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            p.Then(null, fx.f);
            return p;
        }

        [Prototype("finally")]
        public static JSValue Finally(in Arguments a)
        {
            var p = a.This.ToPromise();
            var f = a.Get1();
            if (!(f is JSFunction fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            return p.Then(fx.f, fx.f);
        }
    }
}
