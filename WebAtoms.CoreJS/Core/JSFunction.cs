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

        internal JSFunctionDelegate f;
        internal JSFunction(
            JSFunctionDelegate f,
            string name = null,
            string source = null): base(JSContext.Current?.FunctionPrototype)
        {
            this.f = f;
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
            return source;
        }

        public override JSValue CreateInstance(JSArray args)
        {
            var cx = JSContext.Current;
            JSValue obj = cx.CreateObject();
            obj.prototypeChain = prototype;
            obj = f(obj, args);
            return obj;
        }

        public override JSValue InvokeFunction(JSValue thisValue, JSArray args)
        {
            return f(thisValue, args);
        }

        public static JSValue Call(JSValue receiver, JSArray p)
        {
            return receiver.InvokeFunction(p[0], p.Slice(1));
        }

        public static JSValue Apply(JSValue t, JSArray a){
            JSArray ar = a;
            return t.InvokeFunction(ar[0], ar[1] as JSArray);
        }

        public static JSValue Bind(JSValue t, JSArray a) {
            var fOriginal = (JSFunction)t;
            var tx = a[0];
            var fx = new JSFunction((bt, ba) => fOriginal.f(tx, ba));
            return fx;
        }

        public new static JSFunction Create()
        {
            var fx = new JSFunction(JSFunction.empty, "Function");
            var p = fx.prototype;
            p.DefineProperty(KeyStrings.call, JSProperty.Function(Call));
            p.DefineProperty(KeyStrings.apply, JSProperty.Function(Apply));
            p.DefineProperty(KeyStrings.bind, JSProperty.Function(Bind));
            return fx;
        }
    }
}
