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

            var list = f.AllElements.ToList();

            var result = new List<JSValue>(list.Count);

            int i = 0;

            return new JSPromise((resolve, reject) =>
            {

                foreach (var e in list.ToList())
                {
                    if (!(e is JSPromise p))
                        throw JSContext.Current.NewTypeError($"All parameters must be Promise");
                    var item = e;
                    var ni = i++;
                    p.Then((_, __) =>
                    {
                        result[ni] = _;
                        list.Remove(item);
                        if (!list.Any())
                        {
                            resolve(new JSArray(result));
                        }
                        return JSUndefined.Value;
                    });
                    p.Catch((_, __) => {
                        reject(_);
                        return JSUndefined.Value;
                    });
                }
            });
        }
    }
}
