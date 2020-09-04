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
    public partial class JSArray
    {
        [Static("from")]
        public static JSValue StaticFrom(JSValue t,params JSValue[] a)
        {
            var r = new JSArray();
            var f = a.GetAt(0);
            var map = a.GetAt(1);
            switch (f) {
                case JSUndefined u:
                    throw JSContext.Current.Error("undefined is not iterable");
                case JSNull n:
                    throw JSContext.Current.Error("null is not iterable");
                case JSString str:
                    foreach(var ch in str.value)
                    {
                        JSValue item = new JSString(new string(ch, 1));
                        if (map is JSFunction fn)
                        {
                            item = fn.InvokeFunction(t, item);
                        }
                        r.elements[r._length++] = JSProperty.Property(item);
                    }
                    return r;
                case JSArray array:
                    foreach (var ch in array.elements.AllValues())
                    {
                        JSValue item = ch.Value.value;
                        if (map is JSFunction fn)
                        {
                            item = fn.InvokeFunction(t, item);
                        }
                        r.elements[r._length++] = JSProperty.Property(item);
                    }
                    return r;
            }
            return r;
        }

        [Static("isArray")]
        public static JSValue StaticIsArray(JSValue t,params JSValue[] a)
        {
            return a[0] is JSArray ? JSContext.Current.True : JSContext.Current.False;
        }

        [Static("of")]
        public static JSValue StaticOf(JSValue t,params JSValue[] a)
        {
            var r = new JSArray();
            if (a != null)
            {
                foreach (var e in a)
                {
                    r.elements[r._length++] = JSProperty.Property(e);
                }
            }
            return r;
        }

    }
}
