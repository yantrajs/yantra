using System;
using System.Globalization;
using System.Numerics;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Primitive;

namespace YantraJS.Core.BigInt
{
    static class JSBigIntExtensions
    {
        public static BigInteger AsBigIntegerOnly(this JSValue @this)
        {
            return @this is JSBigInt v ? v.value : throw JSBigInt.CannotMix();
        }
    }

    public partial class JSBigInt : JSPrimitive
    {

        public static JSFunction CreateClass(JSContext context, bool register = true)
        {
            var @class = new Constructor(context.FunctionPrototype);
            if (register)
            {
                context[Names.BigInt] = @class;
            }

            @class.FastAddValue(Names.asIntN, new AsIntNMethod(context.FunctionPrototype), JSPropertyAttributes.ConfigurableValue);
            @class.FastAddValue(Names.asUintN, new AsUintNMethod(context.FunctionPrototype), JSPropertyAttributes.ConfigurableValue);

            var prototype = @class.prototype;
            prototype.FastAddValue(Names.toString, new ToStringMethod(context.FunctionPrototype), JSPropertyAttributes.ConfigurableValue);
            prototype.FastAddValue(Names.toLocaleString, new ToLocaleStringMethod(context.FunctionPrototype), JSPropertyAttributes.ConfigurableValue);
            prototype.FastAddValue(Names.valueOf, new ValueOfMethod(context.FunctionPrototype), JSPropertyAttributes.ConfigurableValue);

            return @class;
        }


        public static JSException CannotMix()
        {
            return JSContext.Current.NewTypeError("Cannot mix BigInt and other types, use explicit conversions");
        }

        internal readonly BigInteger value;

        public override bool BooleanValue => value != 0;

        public override double DoubleValue => throw CannotMix();

        public override long BigIntValue => throw CannotMix();

        public JSBigInt(BigInteger value)
        {
            this.value = value;
        }
        public JSBigInt(string stringValue)
        {
            var v = stringValue.TrimEnd('n').Replace("_", "");
            if (!BigInteger.TryParse(v, out var n))
                throw JSContext.Current.NewTypeError($"{stringValue} is not a valid big integer");
            this.value = n;
        }

        public override bool Equals(JSValue value)
        {
            if (value is JSBigInt bigint)
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
            if (value is JSBigInt bigint)
            {
                return bigint;
            }
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
            return (double)this.value == value;
        }


        public override JSValue TypeOf()
        {
            return JSConstants.BigInt;
        }

        protected override JSObject GetPrototype()
        {
            return (JSContext.Current[Names.BigInt] as JSFunction).prototype;
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
            if (type == typeof(object))
            {
                value = this.value;
                return true;
            }
            return base.ConvertTo(type, out value);
        }

        public override JSValue Negate()
        {
            return new JSBigInt(-this.value);
        }

        public override JSValue BitwiseAnd(JSValue value)
        {
            return new JSBigInt(this.value & value.AsBigIntegerOnly());
        }

        public override JSValue BitwiseOr(JSValue value)
        {
            return new JSBigInt(this.value | value.AsBigIntegerOnly());
        }

        public override JSValue BitwiseXor(JSValue value)
        {
            return new JSBigInt(this.value | value.AsBigIntegerOnly());
        }

        public override JSValue LeftShift(JSValue value)
        {
            return new JSBigInt(this.value << (int)value.AsBigIntegerOnly());
        }

        public override JSValue RightShift(JSValue value)
        {
            return new JSBigInt(this.value >> (byte)value.AsBigIntegerOnly());
        }

        public override JSValue UnsignedRightShift(JSValue value)
        {
            return new JSBigInt(this.value >> (int)value.AsBigIntegerOnly());
        }

        public override JSValue Multiply(JSValue value)
        {
            return new JSBigInt(this.value * value.AsBigIntegerOnly());
        }

        public override JSValue Divide(JSValue value)
        {
            return new JSBigInt(this.value / value.AsBigIntegerOnly());
        }

        public override JSValue Subtract(JSValue value)
        {
            return new JSBigInt(this.value - value.AsBigIntegerOnly());
        }

        public override JSValue AddValue(double value)
        {
            throw CannotMix();
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
            if (value is JSBigInt b)
            {
                return new JSBigInt(this.value + b.value);
            }
            if (value.IsBoolean || value.IsNumber)
            {
                throw CannotMix();
            }
            if (value is JSString @string)
            {
                return new JSString(this.value.ToString() + "n" + @string.ToString());
            }
            if (value is JSObject @object)
                return new JSString(this.value + @object.StringValue);
            return new JSBigInt(this.value + value.BigIntValue);
        }

        public override JSValue ValueOf()
        {
            return this;
        }

    }
}
