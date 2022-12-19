using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Core.Primitive;

namespace YantraJS.Core.BigInt
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
        public JSBigInt(string stringValue)
        {
            var v = stringValue.TrimEnd('n').Replace("_", "");
            if (!long.TryParse(v, out var n))
                throw JSContext.Current.NewTypeError($"{stringValue} is not a valid big integer");
            this.value = n;
        }

        public override bool Equals(JSValue value)
        {
            if(value is JSBigInt bigint)
            {
                return this.value == bigint.value;
            }
            var n = (long)value.DoubleValue;
            return this.value == n;
        }

        public override string ToString()
        {
            return value.ToString() + "n";
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException();
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            if (a.Length == 0)
            {
                return new JSBigInt(0);
            }
            var value = a[0];
            if (value.IsNumber)
            {
                return new JSBigInt((long)value.DoubleValue);
            }
            var v = long.Parse(value.ToString());
            return new JSBigInt(v);
        }

        public override bool StrictEquals(JSValue value)
        {
            if (!(value is JSBigInt bigint))
                return false;
            return this.value == bigint.value;
        }

        public override bool EqualsLiteral(string value)
        {
            return this.value.ToString() == value;
        }

        public override bool EqualsLiteral(double value)
        {
            return this.value == value;
        }
        

        public override JSValue TypeOf()
        {
            return JSConstants.BigInt;
        }

        protected override JSObject GetPrototype()
        {
            return JSContext.Current.BigIntPrototype;
        }

        internal override PropertyKey ToKey(bool create = true)
        {
            return (uint)this.value;
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

        public override JSValue AddValue(double value)
        {
            return new JSBigInt(this.value + (long)value);
        }

        public override JSValue AddValue(string value)
        {
            return new JSString(this.value + value);
        }

        public override JSValue AddValue(JSValue value)
        {
            value = value.IsObject ? value.ValueOf() : value;
            if (value is JSPrimitiveObject primitive)
            {
                value = primitive.value;
            }
            if (value is JSString @string)
            {
                return new JSString(this.value + @string.value);
            }
            if (value is JSObject @object)
                return new JSString(this.value + @object.StringValue);
            return new JSBigInt(this.value + value.BigIntValue);
        }
    }
}
