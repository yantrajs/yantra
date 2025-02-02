using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.CodeGen;

namespace YantraJS.Core
{
    public class JSClass: JSFunction
    {

        internal readonly JSFunction super;
        public JSClass(
            JSFunctionDelegate fx, 
            JSFunction super ,
            string name = null,
            string code = null)
            : base( fx ?? super.f ?? JSFunction.empty, name,code)
        {
            this.super = super;
            this.BasePrototypeObject = super;
            this.prototype.BasePrototypeObject = super.prototype;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddConstructor(JSFunction fx)
        {
            this.f = fx.f;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            if (JSContext.NewTarget == null && JSContext.Current.CurrentNewTarget == null)
                throw JSContext.Current.NewTypeError($"{this} is not a function");
            return f(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override JSValue CreateInstance(in Arguments a)
        {
            var @object = new JSObject()
            {
                BasePrototypeObject = this.prototype
            };
            var ao = a.OverrideThis(@object);
            JSContext.Current.CurrentNewTarget = this;
            var @this = f(ao);
            if (!@this.IsUndefined)
            {
                @this.BasePrototypeObject = this.prototype;
                return @this;
            }
            return @object;
        }

    }
}
