using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public class JSObjectPrototype
    {

        [Prototype("propertyIsEnumerable")]
        public static JSValue PropertyIsEnumerable(JSValue t, params JSValue[] a)
        {
            if(!t.TryAsObjectThrowIfNullOrUndefined(out var @object))
            {
                return JSBoolean.False;
            }
            if (a.Length > 0)
            {
                var text = a[0].ToString();
                var px = @object.GetInternalProperty(text, false);
                if (!px.IsEmpty && px.IsEnumerable)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }


        [Prototype("toString")]
        public static JSValue ToString(JSValue t,params JSValue[] a) => new JSString("[object Object]");

        // [Prototype("toLocaleString")]
        // public static JSValue ToLocaleString(JSValue t, params JSValue[] a)


        [GetProperty("__proto__")]
        internal static JSValue PrototypeGet(JSValue t,params JSValue[] a)
        {
            return t.prototypeChain;
        }

        [SetProperty("__proto__")]
        internal static JSValue PrototypeSet(JSValue t,params JSValue[] a)
        {
            var a0 = a[0];
            var o = a0 as JSObject;
            if (o != null)
                t.prototypeChain = o;
            return a0;
        }


        [Prototype("hasOwnProperty")]
        internal static JSValue HasOwnProperty(JSValue t,params JSValue[] a)
        {
            if (!t.TryAsObjectThrowIfNullOrUndefined(out var @object))
                return JSBoolean.False;
            var first = a.Get1();
            var key = first.ToKey();
            if (key.IsUInt)
            {
                if (@object.elements?.HasKey(key.Key) ?? false)
                    return JSBoolean.True;
            }
            if (@object.ownProperties?.HasKey(key.Key) ?? false)
                return JSBoolean.True;
            return JSBoolean.False;
        }

        [Prototype("isPrototypeOf")]
        internal static JSValue IsPrototypeOf(JSValue t,params JSValue[] a)
        {
            throw new NotImplementedException();
        }


    }
}
