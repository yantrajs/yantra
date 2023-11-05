using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    /// <summary>
    /// JSPrimitive class does not hold prototype, prototype is only resolved from
    /// current context when requested first time
    /// 
    /// Boolean, Number, Integer are derived from JSPrimitive
    /// </summary>
    public abstract class JSPrimitive: JSValue
    {
        internal protected void ResolvePrototype() { 
            if (prototypeChain == null)
            {
                BasePrototypeObject = GetPrototype();
            }
        }

        protected abstract JSObject GetPrototype();

        protected JSPrimitive() : base(null)
        {

        }

        protected JSPrimitive(JSObject prototype): base(prototype)
        {

        }

        public override JSValue this[JSSymbol symbol] {
            get
            {
                ResolvePrototype();
                if (prototypeChain == null)
                    return JSUndefined.Value;
                var px = prototypeChain.GetInternalProperty(symbol);
                if (px.IsEmpty)
                {
                    // throw JSContext.Current.NewTypeError($"{name} property not found on {this.GetType().Name}:{this}");
                    return JSUndefined.Value;
                }
                return this.GetValue(px);
            }
            set => base[symbol] = value;
        }

        public override JSValue this[KeyString name]
        {
            get
            {
                ResolvePrototype();
                if (prototypeChain == null)
                    return JSUndefined.Value;
                var px = prototypeChain.GetInternalProperty(name);
                if (px.IsEmpty)
                {
                    // throw JSContext.Current.NewTypeError($"{name} property not found on {this.GetType().Name}:{this}");
                    return JSUndefined.Value;
                }
                return this.GetValue(px);
            }
            set
            {
                // throw new NotSupportedException();
            }
        }

        public override IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            ResolvePrototype();
            return base.GetAllKeys(showEnumerableOnly, inherited);
        }

        internal override JSFunctionDelegate GetMethod(in KeyString key)
        {
            if(prototypeChain == null)
            {
                BasePrototypeObject = GetPrototype();
            }
            return prototypeChain?.GetMethod(key);
        }

        public override JSValue GetPrototypeOf()
        {
            ResolvePrototype();
            return base.GetPrototypeOf();
        }
    }
}
