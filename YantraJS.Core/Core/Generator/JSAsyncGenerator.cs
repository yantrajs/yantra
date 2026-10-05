#nullable enable
using System;
using YantraJS.Core.Core.Generator;
using YantraJS.Core.Generator;

namespace YantraJS.Core.Generator;

public class JSAsyncGenerator: JSObject, IElementEnumerator
{
    private readonly JSGenerator generator;

    public JSAsyncGenerator(JSGenerator generator)
    {
        this.generator = generator;
    }

    public override IElementEnumerator GetAsyncIterator()
    {
        return this;
    }

    public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
    {
        throw new System.NotImplementedException();
    }

    private static JSValue ToPromise(JSGenerator gen, JSValue lastResult)
    {
        try
        {
            if (!gen.MoveNext(lastResult, out var r))
            {
                return null;
            }

            if(r is not JSAsyncValue av)
            {
                return r;
            }

            r = av.Value;
            var then = r[KeyString.then];
            if (then.IsUndefined)
            {
                return new JSPromise(r, JSPromise.PromiseState.Resolved);
            }

            r = r.InvokeMethod(KeyString.then, new JSFunction((in Arguments a) =>
            {
                // return new JSPromise(a.Get1(), JSPromise.PromiseState.Resolved);
                return a.Get1();
            }), new JSFunction((in Arguments a) =>
            {
                gen.Throw(a.Get1());
                return a.Get1();
            }));
            return r;
        }
        catch (Exception ex)
        {
            return new JSPromise(JSError.From(ex), JSPromise.PromiseState.Rejected);
        }
    }

    public bool MoveNext(out JSValue value)
    {
        // value = ToPromise(generator, JSUndefined.Value);
        // return value != null;

        if(!generator.MoveNext(JSUndefined.Value, out var v))
        {
            value = v;
            return false;
        }

        if(v is JSAsyncValue av)
        {

            var pendingPromise = av.Value;
            // we might have more pending promises...
            value = new JSPromise((resolve, reject) => {
                ToPromise(pendingPromise, resolve, new JSFunction((in a) => {
                    reject(a[0] ?? JSUndefined.Value);
                    return JSUndefined.Value;
                }));
            });
        }
        value = v;

        return true;
    }
    private void ToPromise(JSValue pendingPromise, Action<JSValue> resolve, JSFunction reject)
    {
        // nest all promises till you find a non promise value...
        if (!pendingPromise.IsObject)
        {
            resolve(pendingPromise);
            return;
        }
        if (pendingPromise is not JSAsyncValue av)
        {
            resolve(pendingPromise);
            return;
        }
        pendingPromise = av.Value;
        var then = pendingPromise[KeyString.then];
        if (then.IsUndefined)
        {
            resolve(pendingPromise);
            return;
        }
        then.Call(pendingPromise,
            new JSFunction((in a) => {
                ToPromise(a[0] ?? JSUndefined.Value, resolve, reject);
                return JSUndefined.Value;
            }),
            reject
        );
    }

    //private void ToPromise(JSValue pendingPromise, JSFunction resolve, JSFunction reject)
    //{
    //    // nest all promises till you find a non promise value...
    //    if(!pendingPromise.IsObject)
    //    {   
    //        resolve.Call(JSUndefined.Value, pendingPromise);
    //        return;
    //    }
    //    if(pendingPromise is not JSAsyncValue av)
    //    {
    //        resolve.Call(JSUndefined.Value, pendingPromise);
    //        return;
    //    }
    //    pendingPromise = av.Value;
    //    var then = pendingPromise[KeyString.then];
    //    if(then.IsUndefined)
    //    {
    //        resolve.Call(JSUndefined.Value, pendingPromise);
    //        return;
    //    }
    //    then.Call(pendingPromise, resolve, reject);
    //}

    public bool MoveNextOrDefault(out JSValue value, JSValue @default)
    {
        throw new System.NotImplementedException();
    }

    public JSValue NextOrDefault(JSValue @default)
    {
        throw new System.NotImplementedException();
    }
}
