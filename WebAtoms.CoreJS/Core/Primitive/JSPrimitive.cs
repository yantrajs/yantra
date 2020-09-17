using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
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
                return this.GetValue(prototypeChain.GetInternalProperty(name));
            }
            set
            {
                // throw new NotSupportedException();
            }
        }

        internal override IEnumerable<JSValue> GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            ResolvePrototype();
            return base.GetAllKeys(showEnumerableOnly, inherited);
        }

        internal override IEnumerable<JSValue> AllElements => throw new NotImplementedException();
    }
}
