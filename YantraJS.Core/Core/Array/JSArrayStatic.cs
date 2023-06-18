using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using YantraJS;
using YantraJS.Core.Clr;
using YantraJS.Extensions;

namespace YantraJS.Core
{

    public partial class JSArray
    {

        [JSExport("from", Length = 1)]
        public static JSValue StaticFrom(in Arguments a)
        {
            var r = new JSArray();
            var (f, map, mapThis) = a.Get3();
            var t = a.This;
            var en = f.GetElementEnumerator();
            uint length = 0;
            ref var elements = ref r.GetElements();

            if (map is JSFunction fx)
            {
                var cb = fx.f;
                while (en.MoveNext(out var hasValue, out var item, out var index))
                {
                    elements.Put(length++, cb(new Arguments(mapThis, item,new JSNumber(index))));
                }
            }
            else
            {
                while (en.MoveNext(out var hasValue, out var item, out var index))
                {
                    elements.Put(length++, item);
                }
            }
            r._length = length;
            return r;
        }

        [JSExport("isArray", Length = 1)]
        public static JSValue StaticIsArray(in Arguments a)
        {
            return a.Get1() is JSArray ? JSBoolean.True : JSBoolean.False;
        }

        [JSExport("of")]
        public static JSValue StaticOf(in Arguments a)
        {
            var r = new JSArray();
            var al = a.Length;
            ref var rElements = ref r.CreateElements();
            for(var ai = 0; ai<al; ai++)
            {
                rElements.Put(r._length++, a.GetAt(ai));
            }
            return r;
        }

    }
}
