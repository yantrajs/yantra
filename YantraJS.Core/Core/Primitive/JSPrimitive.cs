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

        protected void ResolvePrototype() { 
            if (prototypeChain == null)
            {
                prototypeChain = GetPrototype();
            }
        }

        protected abstract JSObject GetPrototype();

        protected JSPrimitive() : base(null)
        {

        }

        public override JSValue this[KeyString name]
        {
            get
            {
                ResolvePrototype();
                var px = prototypeChain.GetInternalProperty(name);
                if (px.IsEmpty)
                {
                    throw JSContext.Current.NewTypeError($"{name} property not found on {this}");
                }
                return this.GetValue(px);
            }
            set
            {
                // throw new NotSupportedException();
            }
        }

        internal override IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            ResolvePrototype();
            return base.GetAllKeys(showEnumerableOnly, inherited);
        }

    }
}
