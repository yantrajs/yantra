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

    public bool MoveNext(out JSValue value)
    {
        // value = ToPromise(generator, JSUndefined.Value);
        // return value != null;

        if(!generator.MoveNext(JSUndefined.Value, out var v))
        {
            value = v;
            return false;
        }

        value = new JSPromise((resolve, reject) => {
                ToPromise(v, resolve, new JSFunction((in a) => {
                    reject(a[0] ?? JSUndefined.Value);
                    return JSUndefined.Value;
                }));
            });

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

                generator.MoveNext(JSUndefined.Value, out var v);

                ToPromise(v, resolve, reject);
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
