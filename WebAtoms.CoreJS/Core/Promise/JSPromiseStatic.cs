using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core.Runtime
{
    public static class JSPromiseStatic
    {
        [Static("all")]
        public static JSValue All(JSValue t, JSValue[] a)
        {
            var f = a.GetAt(0);
            if (f.IsUndefined)
                throw JSContext.Current.NewTypeError($"The parameter must be an iterable");

            var result = new List<JSValue>();

            int i = 0;

            return new JSPromise((resolve, reject) =>
            {
                int total = 0;

                bool empty = true;                

                foreach (var e in f.AllElements)
                {
                    empty = false;
                    if (!(e is JSPromise p))
                        throw JSContext.Current.NewTypeError($"All parameters must be Promise");
                    var item = e;
                    var ni = i++;
                    total = i;
                    
                    p.Then((in Arguments args) =>
                    {
                        var r = args.Get1();
                        result[ni] = r;
                        total--;
                        if (total <= 0)
                        {
                            resolve(new JSArray(result));
                        }
                        return JSUndefined.Value;
                    });
                    p.Catch((in Arguments args) => {
                        reject(args.Get1());
                        return JSUndefined.Value;
                    });
                }

                if (empty)
                {
                    resolve(new JSArray());
                }
            });
        }
    }
}
