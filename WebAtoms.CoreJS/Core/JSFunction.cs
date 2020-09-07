using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSFunction : JSObject
    {

        internal static JSFunctionDelegate empty = (_, __) => _;

        internal readonly JSObject prototype;

        private string source;

        private string name;

        internal JSFunctionDelegate f;
        public JSFunction(
            JSFunctionDelegate f,
            string name = null,
            string source = null): base(JSContext.Current?.FunctionPrototype)
        {
            //this.f = (t,a) => {
            //    using (JSContext.Current.Scope.NewScope())
            //    {
            //        return f(t, a);
            //    }
            //};
            this.f = f;
            this.name = name ?? "native";
            this.source = source 
                ?? $"function {name ?? "native"}() {{ [native] }}";
            prototype = new JSObject();
            prototype[KeyStrings.constructor] = this;
            this[KeyStrings.prototype] = prototype;

            this[KeyStrings.name] = name != null
                ? new JSString(name)
                : new JSString("native");

        }

        public override string ToString()
        {
            return name;
        }

        public override string ToDetailString()
        {
            return this.source;
        }

        public override JSValue CreateInstance(JSValue[] args)
        {
            var cx = JSContext.Current;
            JSValue obj = cx.CreateObject();
            obj.prototypeChain = prototype;
            obj = f(obj, args);
            return obj;
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            return f(thisValue, args);
        }

        [Prototype("call")]
        public static JSValue Call(JSValue receiver, params JSValue[] p)
        {
            var (first, args) = p.Slice();
            return receiver.InvokeFunction(first, args);
        }

        [Prototype("apply")]
        public static JSValue Apply(JSValue t,params JSValue[] a){
            var ar = a;
            return t.InvokeFunction(ar[0], new JSArguments(ar[1] as JSArray));
        }

        [Prototype("bind")]
        public static JSValue Bind(JSValue t,params JSValue[] a) {
            var fOriginal = (JSFunction)t;
            var tx = a[0];
            var fx = new JSFunction((bt, ba) => fOriginal.f(tx, ba));
            return fx;
        }

        //[Prototype("prototype", MemberType.Get)]
        //internal static JSValue GetPrototype(JSValue t,params JSValue[] a)
        //{
        //    if (t is JSFunction fx)
        //        return fx.prototype;
        //    return JSUndefined.Value;
        //}

        //[Prototype("prototype", MemberType.Set)]
        //internal static JSValue SetPrototype(JSValue t,params JSValue[] a)
        //{
        //    if (t is JSFunction fx)
        //    {
        //        var px = a[0];
        //        fx.prototype = px as JSObject;
        //        return px;
        //    }
        //    return JSUndefined.Value;
        //}
    }
}
