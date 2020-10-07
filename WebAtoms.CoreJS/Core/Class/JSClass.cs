using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core.Class
{
    public class JSClass: JSFunction
    {

        internal readonly JSFunction super;
        public JSClass(JSFunctionDelegate fx, JSFunction super = null)
            : base(fx)
        {
            this.super = super;
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            var @object = super?.CreateInstance(a) ?? new JSObject(this);
            var ao = a.OverrideThis(@object);
            var @this = f(ao);
            @this.prototypeChain = this.prototype;
            return @this;
        }

    }
}
