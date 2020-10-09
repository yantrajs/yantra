using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            this.prototypeChain = super;
            this.prototype.prototypeChain = super.prototype;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"{this} is not a function");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override JSValue CreateInstance(in Arguments a)
        {
            var @object = new JSObject();
            var ao = a.OverrideThis(@object);
            var @this = (f ?? super.f)(ao);
            if (@this.IsUndefined)
                @this = @object;
            @this.prototypeChain = this.prototype;
            return @this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSClass AddPrototypeProperty(KeyString name, JSFunction getter, JSFunction setter)
        {
            this.prototype.ownProperties[name.Key] = JSProperty.Property(name, getter.f, setter?.f, JSPropertyAttributes.ConfigurableProperty);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSClass AddPrototypeMethod(KeyString name, JSValue value)
        {
            this.prototype.ownProperties[name.Key] = JSProperty.Property(name, value, JSPropertyAttributes.ConfigurableValue);
            return this;
        }

    }
}
