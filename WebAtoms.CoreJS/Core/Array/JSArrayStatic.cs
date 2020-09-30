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
            if (f.IsUndefined)
                throw JSContext.Current.NewError("undefined is not iterable");
            if (f.IsNull)
                throw JSContext.Current.NewError("null is not iterable");
            switch (f)
            {
                case JSString str:
                    foreach (var ch in str.value)
                    {
                        JSValue item = new JSString(new string(ch, 1));
                        if (map is JSFunction fn)
                        {
                            item = fn.InvokeFunction(new Arguments(t, item));
                        }
                        r.elements[r._length++] = JSProperty.Property(item);
                    }
                    return r;
                case JSArray array:
                    foreach (var ch in array.GetArrayElements())
                    {
                        JSValue item = ch.value;
                        if (map is JSFunction fn)
                        {
                            item = fn.InvokeFunction(new Arguments(t, item));
                        }
                        r.elements[r._length++] = JSProperty.Property(item);
                    }
                    return r;
            }
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
            var en = new Arguments.ArgumentsEnumerator(a);
            while(en.MoveNext())
            {
                r.elements[r._length++] = JSProperty.Property(en.Current);
            }
            return r;
        }

    }
}
