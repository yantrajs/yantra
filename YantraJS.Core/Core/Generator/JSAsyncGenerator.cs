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

            if(r is JSAsyncValue av)
            {
                return av.Value;
                // retr
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
        }
        catch (Exception ex)
        {
            return new JSPromise(JSError.From(ex), JSPromise.PromiseState.Rejected);
        }
    }

    public bool MoveNext(out JSValue value)
    {
        value = ToPromise(generator, JSUndefined.Value);
        return value != null;
    }

    public bool MoveNextOrDefault(out JSValue value, JSValue @default)
    {
        throw new System.NotImplementedException();
    }

    public JSValue NextOrDefault(JSValue @default)
    {
        throw new System.NotImplementedException();
    }
}
