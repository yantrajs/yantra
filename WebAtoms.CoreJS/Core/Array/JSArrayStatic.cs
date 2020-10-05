using Esprima.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{

    public class JSArrayStatic
    {

        [Static("from")]
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
                while (en.MoveNext())
                {
                    elements[length++] = JSProperty.Property(cb(new Arguments(t, en.Current)));
                }
            }
            else
            {
                while (en.MoveNext())
                {
                    elements[length++] = JSProperty.Property(en.Current);
                }
            }
            r._length = length;
            return r;
        }

        [Static("isArray")]
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
