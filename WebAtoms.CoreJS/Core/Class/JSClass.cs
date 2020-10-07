using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSClass: JSFunction
    {

        internal readonly JSFunction super;
        public JSClass(
            JSFunctionDelegate fx, 
            JSFunction super ,
            string name = null,
            string code = null)
            : base(fx ?? JSFunction.empty, name, code)
        {
            this.super = super;
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            var @object = super?.CreateInstance(a) ?? new JSObject(this.prototype);
            var ao = a.OverrideThis(@object);
            var @this = f(ao);
            @this.prototypeChain = this.prototype;
            return @this;
        }

        internal JSClass AddPrototypeProperty(KeyString name, JSFunction getter, JSFunction setter)
        {
            this.prototype.AddProperty(name, getter, setter);
            return this;
        }

        internal JSClass AddPrototypeMethod(KeyString name, JSFunction value)
        {
            this.prototype.AddProperty(name, value);
            return this;
        }

    }
}
