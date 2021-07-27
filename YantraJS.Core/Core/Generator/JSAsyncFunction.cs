#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Generator;
using YantraJS.Core.LinqExpressions.GeneratorsV2;

namespace YantraJS.Core.Core.Generator
{
    public class JSAsyncFunction
    {

        public static JSFunction Create(JSGeneratorFunctionV2 gf)
        {
            JSValue ToAsync(in Arguments a)
            {
                var gen = gf.InvokeFunction(in a) as JSGenerator;

                return ToPromise(gen!, JSUndefined.Value);
            }

            return new JSFunction(ToAsync, gf.name, gf.Length);
        }

        private static JSValue ToPromise(JSGenerator gen, JSValue lastResult)
        {
            try
            {
                if(!gen.MoveNext(lastResult, out var r))
                {
                    return new JSPromise(r, JSPromise.PromiseState.Resolved);
                }

                var then = r[KeyStrings.then];
                if (then.IsUndefined)
                {
                    return new JSPromise(r, JSPromise.PromiseState.Resolved);
                }

                r = r.InvokeMethod(in KeyStrings.then, new JSFunction((in Arguments a) =>
                {
                    return ToPromise(gen, a.Get1());
                }), new JSFunction((in Arguments a) =>
                {
                    gen.Throw(a.Get1());
                    return a.Get1();
                }));
                return r;
            } catch (Exception ex)
            {
                return new JSPromise(JSError.From(ex), JSPromise.PromiseState.Rejected);
            }
        }
    }
}
