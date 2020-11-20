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
            var (success, fail) = a.Get2();
            if (!(success is JSFunction successFx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            if (!fail.IsUndefined)
            {
                if (!(fail is JSFunction failFx))
                    throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
                return p.Then(a.Context, successFx.f, failFx.f);
            }
            return p.Then(a.Context, successFx.f, null);
        }

        [Prototype("catch")]
        public static JSValue Catch(in Arguments a)
        {
            var p = a.This.ToPromise();
            var f = a.Get1();
            if (!(f is JSFunction fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            p.Then(a.Context, null, fx.f);
            return p;
        }

        [Prototype("finally")]
        public static JSValue Finally(in Arguments a)
        {
            var p = a.This.ToPromise();
            var f = a.Get1();
            if (!(f is JSFunction fx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            return p.Then(a.Context, fx.f, fx.f);
        }
    }
}
