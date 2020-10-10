using Esprima.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using WebAtoms.CoreJS;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{

    public class JSArrayStatic
    {

        [Static("from", Length = 1)]
        public static JSValue StaticFrom(in Arguments a)
        {
            var r = new JSArray();
            var (f, map) = a.Get2();
            var t = a.This;
            var en = f.GetElementEnumerator();
            uint length = 0;
            var elements = r.elements;

            if (map is JSFunction fx)
            {
                var cb = fx.f;
                while (en.MoveNext(out var hasValue, out var item, out var index))
                {
                    elements[length++] = JSProperty.Property(cb(new Arguments(t, item)));
                }
            }
            else
            {
                while (en.MoveNext(out var hasValue, out var item, out var index))
                {
                    elements[length++] = JSProperty.Property(item);
                }
            }
            r._length = length;
            return r;
        }

        [Static("isArray", Length = 1)]
        public static JSValue StaticIsArray(in Arguments a)
        {
            return a.Get1() is JSArray ? JSBoolean.True : JSBoolean.False;
        }

        [Static("of")]
        public static JSValue StaticOf(in Arguments a)
        {
            var r = new JSArray();
            var al = a.Length;
            for(var ai = 0; ai<al; ai++)
            {
                r.elements[r._length++] = JSProperty.Property(a.GetAt(ai));
            }
            return r;
        }

    }
}
