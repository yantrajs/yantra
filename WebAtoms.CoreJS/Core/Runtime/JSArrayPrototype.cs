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
        [Prototype("push")]
        public static JSValue Push (JSValue t,params JSValue[] a){
            var ta = (JSArray)t;
            foreach(var item in a)
            {
                ta.elements[ta._length] = JSProperty.Property(item);
                ta._length++;
            }
            return new JSNumber(ta._length);
        }

        [Prototype("pop")]
        public static JSValue Pop(JSValue t,params JSValue[] a)
        {
            var ta = (JSArray)t;
            if (ta._length == 0)
                return JSUndefined.Value;
            JSProperty r;
            if (ta.elements.TryRemove(ta._length - 1, out r))
            {
                ta._length--;
                return r.value;
            }
            return JSUndefined.Value;
        }

        [Prototype("slice")]
        public static JSArray Slice(JSValue t,params JSValue[] a){
            var ta = (JSArray)t;
            var start = a.TryGetAt(0, out var a0) ? a0.IntValue : 0;
            var end = a.TryGetAt(1, out var a1) ? a1.IntValue : -1;
            return ta.Slice(start, end);
        }

        [GetProperty("length")]
        internal static JSValue GetLength(JSValue t,params JSValue[] a)
        {
            return new JSNumber(((JSArray)t)._length);
        }

        [SetProperty("length")]
        internal static JSValue SetLength(JSValue t,params JSValue[] a)
        {
            return new JSNumber(((JSArray)t)._length = (uint)a[0].IntValue);
        }



    }
}
