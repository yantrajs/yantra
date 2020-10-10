using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public class JSProxy : JSValue
    {
        readonly JSObject target;
        protected JSProxy(JSObject target, JSObject handler) : base(null)
        {
            if (target == null || handler == null)
            {
                throw JSContext.Current.NewTypeError("Cannot create proxy with a non-object as target or handler");
            }
            this.target = target;
        }

        public override bool BooleanValue => target.BooleanValue;


        public override JSBoolean Equals(JSValue value)
        {
            return target.Equals(value);
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            return target.InvokeFunction(a);
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            return target.StrictEquals(value);
        }

        public override JSValue TypeOf()
        {
            return target.TypeOf();
        }

        internal override KeyString ToKey(bool create = false)
        {
            return target.ToKey();
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            var (f, s) = a.Get2();
            return new JSProxy(f as JSObject, s as JSObject);
        }
    }
}
