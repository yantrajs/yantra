#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Core.Generator;
using YantraJS.Core.LinqExpressions.GeneratorsV2;

namespace YantraJS.Core.Core.Generator
{
    public class JSAsyncValue: JSObject
    {
        public readonly JSValue Value;

        public JSAsyncValue(JSValue value)
        {
            this.Value = value;
        }
    }

    // need generator changes here...
    public class JSAsyncFunction
    {

        public static JSFunction Create(JSGeneratorFunctionV2 gf)
        {
            JSValue ToAsync(in Arguments a)
            {
                var gen = gf.InvokeFunction(in a) as JSGenerator;

                // return ToPromise(gen!, JSUndefined.Value);

                var p = new JSPromise((resolve, reject) => {
                    ToPromise2(gen, JSUndefined.Value, resolve, new JSFunction((in a) => {
                        reject(a[0] ?? JSUndefined.Value);
                        return JSUndefined.Value;
                    }));
                });
                return p; 
            }

            return new JSFunction(ToAsync, gf.name, gf.Length);
        }

        private static void ToPromise2(JSGenerator gen, JSValue value, Action<JSValue> resolve, JSFunction reject)
        {
            Console.WriteLine($"ToPromise {value}");
            do
            {
                if (!gen.MoveNext(value, out var r))
                {
                    resolve(value);
                    return;
                }
                if(r.IsNullOrUndefined || !r.IsObject)
                {
                    continue;
                }
                var then = r[KeyString.then];
                if (then.IsNullOrUndefined)
                {
                    continue;
                }
                then.Call(r, new JSFunction((in a) => {
                    ToPromise2(gen, a[0] ?? r ?? value, resolve, reject);
                    return JSUndefined.Value;
                }), reject);
                break;

            } while (true);
            // we need to iterate till end...
        }

        public static JSFunction CreateGenerator(JSGeneratorFunctionV2 gf)
        {
            JSValue ToAsync(in Arguments a)
            {
                return new JSAsyncGenerator(gf.InvokeFunction(in a) as JSGenerator);

                // return ToPromise(gen!, JSUndefined.Value);
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
                if(!r.IsObject)
                {
                    return new JSPromise(r, JSPromise.PromiseState.Resolved);
                }
                var then = r[KeyString.then];
                if (then.IsUndefined)
                {
                    return new JSPromise(r, JSPromise.PromiseState.Resolved);
                }

                r = r.InvokeMethod(KeyString.then, new JSFunction((in Arguments a) =>
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
