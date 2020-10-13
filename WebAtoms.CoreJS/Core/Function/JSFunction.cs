using Esprima.Ast;
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

        internal JSObject prototype;

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
            string source = null,
            int length = 0): base(JSContext.Current?.FunctionPrototype)
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
            this[KeyStrings.length] = new JSNumber(length);

        }

        public override JSValue this[KeyString name] { 
            get => base[name]; 
            set {
                if (name.Key == KeyStrings.prototype.Key)
                {
                    this.prototype = value as JSObject;
                }
                base[name] = value;
            }
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
            var a1 = a.OverrideThis(obj);
            var r = f(a1);
            if (!r.IsUndefined)
                return r;
            return obj;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            return f(a);
        }

        [Prototype("call", Length = 1)]
        public static JSValue Call(in Arguments a)
        {
            var a1 = a.CopyForCall();
            return a.This.InvokeFunction(a1);
        }

        [Prototype("apply", Length = 2)]
        public static JSValue Apply(in Arguments a){
            var ar = a.CopyForApply();
            return a.This.InvokeFunction(ar);
        }

        [Prototype("bind", Length = 1)]
        public static JSValue Bind(in Arguments a) {
            var fOriginal = a.This as JSFunction;
            var original = a;
            var fx = new JSFunction((in Arguments a2) =>
            {
                if (a2.Length == 0)
                {
                    // for constructor...
                    return fOriginal.f(original);
                }
                return fOriginal.f(a2.OverrideThis(original.Get1()));
            });
            // need to set prototypeChain...
            fx.prototypeChain = fOriginal;
            return fx;
        }

        internal static JSValue InvokeSuperConstructor(JSValue super, in Arguments a)
        {
            var @this = a.This;
            var r = (super as JSFunction).f(a);
            return r.IsUndefined ? @this : r;
        }
    }
}
