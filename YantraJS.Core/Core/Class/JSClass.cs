using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.CodeGen;

namespace YantraJS.Core
{
    public class JSClass: JSClosureFunction
    {

        internal readonly JSFunction super;
        public JSClass(
            ScriptInfo script,
            JSVariable[] closures,
            JSClosureFunctionDelegate fx, 
            JSFunction super ,
            string name = null,
            string code = null)
            : base(script, closures, GetFactory(fx,super), name, code)
        {
            this.super = super;
            this.BasePrototypeObject = super;
            this.prototype.BasePrototypeObject = super.prototype;
        }

        private static JSClosureFunctionDelegate GetFactory(JSClosureFunctionDelegate fx, JSFunction super)
        {
            if (fx != null)
                return fx;
            if (super != null)
            {
                var f = super.f;
                JSValue CallSuper(ScriptInfo script, JSVariable[] vars, in Arguments a)
                {
                    return f(in a);
                }
                return CallSuper;
            }
            return JSFunction.emptyCF;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            if (a.NewTarget == null)
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
            var ao = a.OverrideThis(@object, this);
            var @this = f(ao);
            if (@this.IsUndefined)
                @this = @object;
            return @this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSClass AddPrototypeProperty(in KeyString name, JSFunction getter, JSFunction setter)
        {
            this.prototype.GetOwnProperties()[name.Key] = JSProperty.Property(name, getter.f, setter?.f, JSPropertyAttributes.ConfigurableProperty);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSClass AddPrototypeMethod(in KeyString name, JSValue value)
        {
            this.prototype.GetOwnProperties()[name.Key] = JSProperty.Property(name, value, JSPropertyAttributes.ConfigurableValue);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSClass AddPrototypeMethod(JSValue name, JSValue value)
        {
            var key = name.ToKey();
            if (key.IsSymbol)
            {
                this.prototype.DefineProperty(key.JSValue as JSSymbol, JSProperty.Property(key, value, JSPropertyAttributes.ConfigurableValue));
                return this;
            }
            this.prototype.DefineProperty(key, JSProperty.Property(key, value, JSPropertyAttributes.ConfigurableValue));
            return this;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSClass AddStaticProperty(in KeyString name, JSFunction getter, JSFunction setter)
        {
            ref var ownProperties = ref this.GetOwnProperties();
            ownProperties[name.Key] = JSProperty.Property(name, getter.f, setter?.f, JSPropertyAttributes.ConfigurableProperty);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSClass AddStaticMethod(in KeyString name, JSValue value)
        {
            ref var ownProperties = ref this.GetOwnProperties();
            ownProperties[name.Key] = JSProperty.Property(name, value, JSPropertyAttributes.ConfigurableValue);
            return this;
        }


    }
}
