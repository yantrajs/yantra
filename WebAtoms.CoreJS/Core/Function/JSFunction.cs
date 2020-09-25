using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSFunction : JSObject
    {

        internal static JSFunctionDelegate empty = (in Arguments a) => a.This;

        internal readonly JSObject prototype;

        private string source;

        private string name;

        internal JSFunctionDelegate f;

        public override bool IsFunction => true;

        public override JSValue TypeOf()
        {
            return JSConstants.Function;
        }
        public JSFunction(
            JSFunctionDelegate f,
            string name = null,
            string source = null): base(JSContext.Current?.FunctionPrototype)
        {
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

        public override JSValue CreateInstance(in Arguments a)
        {
            JSValue obj = new JSObject();
            obj.prototypeChain = prototype;
            var r = f(a);
            if (!r.IsUndefined)
                return r;
            return obj;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            return f(a);
        }

        [Prototype("call")]
        public static JSValue Call(in Arguments a)
        {
            var a1 = a.CopyForCall();
            return a.This.InvokeFunction(a1);
        }

        [Prototype("apply")]
        public static JSValue Apply(in Arguments a){
            var ar = a.CopyForApply();
            return a.This.InvokeFunction(ar);
        }

        [Prototype("bind")]
        public static JSValue Bind(in Arguments a) {
            var fOriginal = a.This as JSFunction;
            var a1 = a.OverrideThis(a.This);
            var fx = new JSFunction((in Arguments a2) => fOriginal.f(a2));
            return fx;
        }
    }
}
