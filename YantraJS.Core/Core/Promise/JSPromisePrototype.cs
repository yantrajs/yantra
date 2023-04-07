using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;

namespace YantraJS.Core
{

    public partial class JSPromise
    {


        [JSExport("then")]
        public JSValue Then(in Arguments a)
        {
            var (success, fail) = a.Get2();
            if (!(success is JSFunction successFx))
                throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
            if (!fail.IsUndefined)
            {
                if (!(fail is JSFunction failFx))
                    throw JSContext.Current.NewTypeError($"Parameter for then is not a function");
                return Then(successFx.f, failFx.f);
            }
            return Then(successFx.f, null);
        }

        [JSExport("catch")]
        public JSValue Catch(JSFunction fx)
        {
            Then(null, fx.f);
            return this;
        }

        [JSExport("finally")]
        public JSValue Finally(JSFunction fx)
        {
            return Then(fx.f, fx.f);
        }
    }
}
