using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{
    public class JSObjectPrototype
    {

        [Prototype("propertyIsEnumerable")]
        public static JSValue PropertyIsEnumerable(in Arguments a)
        {
            if(!a.This.TryAsObjectThrowIfNullOrUndefined(out var @object))
            {
                return JSBoolean.False;
            }
            if (a.Length > 0)
            {
                var text = a.Get1().ToString();
                ref var px = ref @object.GetInternalProperty(text, false);
                if (!px.IsEmpty && px.IsEnumerable)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [Prototype("toString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "JavaScript Method Signature is Standard")]
        public static JSValue ToString(in Arguments a) => new JSString("[object Object]");

        // [Prototype("toLocaleString")]
        // public static JSValue ToLocaleString(JSValue t, params JSValue[] a)


        [GetProperty("__proto__")]
        internal static JSValue PrototypeGet(in Arguments a)
        {
            return a.This.prototypeChain?.@object ?? JSNull.Value;
        }

        [SetProperty("__proto__")]
        internal static JSValue PrototypeSet(in Arguments a)
        {
            var a0 = a.Get1();
            if (a0 is JSObject o)
            {
                a.This.BasePrototypeObject = o;
            }
            return a0;
        }


        [Prototype("hasOwnProperty")]
        internal static JSValue HasOwnProperty(in Arguments a)
        {
            if (!a.This.TryAsObjectThrowIfNullOrUndefined(out var @object))
                return JSBoolean.False;
            var first = a.Get1();
            var key = first.ToKey(false);
            if (key.IsUInt)
            {
                ref var elements = ref @object.GetElements();
                if (elements.HasKey(key.Key))
                    return JSBoolean.True;
            }
            ref var op = ref @object.GetOwnProperties(false);
            if (op.HasKey(key.Key))
                return JSBoolean.True;
            return JSBoolean.False;
        }

        [Prototype("isPrototypeOf")]
        internal static JSValue IsPrototypeOf(in Arguments a)
        {
            if (!a.This.TryAsObjectThrowIfNullOrUndefined(out var @this))
                return JSBoolean.False;
            var first = a.Get1();
            while (true)
            {
                if (@this == first.prototypeChain?.@object)
                    return JSBoolean.True;
                if (first.prototypeChain?.@object == first || first.prototypeChain?.@object == null)
                    break;
                first = first.prototypeChain?.@object;
            }
            return JSBoolean.False;
        }


    }
}
