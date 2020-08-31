using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSFunction : JSObject
    {

        internal static JSFunctionDelegate empty = (_, __) => JSUndefined.Value;

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

        public override JSValue CreateInstance(JSArguments args)
        {
            var cx = JSContext.Current;
            JSValue obj = cx.CreateObject();
            obj.prototypeChain = prototype;
            obj = f(obj, args);
            return obj;
        }

        public override JSValue InvokeFunction(JSValue thisValue, JSArguments args)
        {
            return f(thisValue, args);
        }

        [Prototype("call")]
        public static JSValue Call(JSValue receiver, JSArguments p)
        {
            var (first, args) = p.Slice();
            return receiver.InvokeFunction(first, args);
        }

        [Prototype("apply")]
        public static JSValue Apply(JSValue t, JSArguments a){
            var ar = a;
            return t.InvokeFunction(ar[0], new JSArguments(ar[1] as JSArray));
        }

        [Prototype("bind")]
        public static JSValue Bind(JSValue t, JSArguments a) {
            var fOriginal = (JSFunction)t;
            var tx = a[0];
            var fx = new JSFunction((bt, ba) => fOriginal.f(tx, ba));
            return fx;
        }
    }
}
