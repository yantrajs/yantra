﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using YantraJS.Extensions;

namespace YantraJS.Core.Runtime
{
    public static class JSPromiseStatic
    {
        [Static("resolve")]
        public static JSValue Resolve(in Arguments a)
        {
            return new JSPromise(a.Get1(), JSPromise.PromiseState.Resolved);
        }

        [Static("reject")]
        public static JSValue Reject(in Arguments a)
        {
            var reason = a.Get1();
            if(reason.IsNullOrUndefined)
            {
                throw JSContext.Current.NewTypeError($"Failure reason must be provided for rejected promise");
            }
            return new JSPromise(reason, JSPromise.PromiseState.Rejected);
        }


        [Static("all")]
        public static JSValue All(in Arguments a)
        {
            var f = a.Get1();

            var en = f.GetElementEnumerator();

            var result = new JSArray();

            uint i = 0;

            return new JSPromise((resolve, reject) =>
            {
                var sc = JSContext.Current.synchronizationContext;
                if (sc == null)
                    throw JSContext.Current.NewTypeError($"Cannot use promise without Synchronization Context");

                uint total = 0;

                bool empty = true;   

                while(en.MoveNext(out var hasValue, out var e, out var index))
                {
                    empty = false;
                    if (!(e is JSPromise p))
                        throw JSContext.Current.NewTypeError($"All parameters must be Promise");
                    var item = e;
                    var ni = i++;
                    total = i;
                    
                    p.Then((in Arguments args) =>
                    {
                        var r1 = args.Get1();
                        sc.Post((r) => { 
                        result[ni] = r as JSValue;
                        total--;
                        if (total <= 0)
                        {
                            resolve(result);
                        }
                        }, r1);
                        return JSUndefined.Value;
                    }, (in Arguments args) => {
                        var v = args.Get1();
                        sc.Post((o) => { 
                            reject(o as JSValue);
                        }, v);
                        return JSUndefined.Value;
                    });
                }

                if (empty)
                {
                    sc.Post((o) => {
                        resolve(new JSArray());
                    }, null);
                }
            });
        }
    }
}
