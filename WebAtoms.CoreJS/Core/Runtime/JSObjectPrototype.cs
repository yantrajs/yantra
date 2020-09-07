using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSObject
    {

        [Prototype("propertyIsEnumerable")]
        public static JSValue PropertyIsEnumerable(JSValue t, params JSValue[] a)
        {
            switch (t)
            {
                case JSUndefined _:
                case JSNull _:
                    throw JSContext.Current.NewError("Cannot convert undefined or null to object");
            }
            if (a.Length > 0)
            {
                var text = a[0].ToString();
                var px = t.GetInternalProperty(text, false);
                if (!px.IsEmpty && px.IsEnumerable)
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }


        [Prototype("toString")]
        public static JSValue ToString(JSValue t,params JSValue[] a) => new JSString("[object Object]");


        [GetProperty("__proto__")]
        internal static JSValue PrototypeGet(JSValue t,params JSValue[] a)
        {
            return t.prototypeChain;
        }

        [SetProperty("__proto__")]
        internal static JSValue PrototypeSet(JSValue t,params JSValue[] a)
        {
            return t.prototypeChain = a[0];
        }


        [Prototype("hasOwnProperty")]
        internal static JSValue HasOwnProperty(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Prototype("isPrototypeOf")]
        internal static JSValue IsPrototypeOf(JSValue t,params JSValue[] a)
        {
            return t;
        }


    }
}
