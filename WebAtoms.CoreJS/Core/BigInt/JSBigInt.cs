using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core.BigInt
{
    [JSRuntime(typeof(JSBigIntStatic), typeof(JSBigIntPrototype))]
    public class JSBigInt : JSPrimitive
    {

        internal readonly long value;

        public override bool BooleanValue => value != 0;

        public override double DoubleValue => value;

        public override long BigIntValue => value; 

        public JSBigInt(long value)
        {
            this.value = value;
        }

        public override JSBoolean Equals(JSValue value)
        {
            if(value is JSBigInt bigint)
            {
                return this.value == bigint.value ? JSBoolean.True : JSBoolean.False;
            }
            var n = (long)value.DoubleValue;
            return this.value == n ? JSBoolean.False : JSBoolean.False;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException();
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (!(value is JSBigInt bigint))
                return JSBoolean.False;
            return this.value == bigint.value ? JSBoolean.True : JSBoolean.False;
        }

        public override JSValue TypeOf()
        {
            return JSConstants.BigInt;
        }

        protected override JSObject GetPrototype()
        {
            return JSContext.Current.BigIntPrototype;
        }

        internal override KeyString ToKey(bool create = true)
        {
            return new KeyString(null, (uint)this.value);
        }

        public override bool ConvertTo(Type type, out object value)
        {            
            if (type == typeof(long))
            {
                value = this.value;
                return true;
            }
            if (type == typeof(ulong))
            {
                value = (ulong)this.value;
                return true;
            }
            if (type.IsAssignableFrom(typeof(JSBigInt)))
            {
                value = this;
                return true;
            }
            if (type  == typeof(object))
            {
                value = this.value;
                return true;
            }
            return base.ConvertTo(type, out value);
        }
    }
}
