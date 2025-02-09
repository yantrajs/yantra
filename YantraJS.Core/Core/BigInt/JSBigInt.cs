using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Yantra.Core;
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

    // [JSRuntime(typeof(JSBigIntStatic), typeof(JSBigIntPrototype))]
    [JSBaseClass("Object")]
    [JSFunctionGenerator("BigInt")]
    [JSFunctionConstructorField]
    public partial class JSBigInt : JSPrimitive
    {


        public static JSException CannotMix()
        {
            return JSContext.Current.NewTypeError("Cannot mix BigInt and other types, use explicit conversions");
        }

        internal readonly BigInteger value;

        public override bool BooleanValue => value != 0;

        public override double DoubleValue => throw CannotMix();

        public override long BigIntValue => throw CannotMix();

        [JSExport(IsConstructor = true)]
        public static JSValue Constructor(in Arguments a)
        {
            var f = a[0];
            switch (f)
            {
                case JSNumber number:
                    return new JSBigInt((BigInteger)number.value);
                case JSBigInt bigint:
                    return bigint;
            }
            var text = f.ToString();
            text = text.TrimEnd('n').Replace("_", "");
            if (!BigInteger.TryParse(text, out var v))
            {
                throw JSContext.Current.NewTypeError($"{f} is not a valid big integer");
            }
            return new JSBigInt(v);

        }

        public JSBigInt(BigInteger value)
        {
            this.value = value;
        }

        public JSBigInt(JSObject prototype, string stringValue): base(prototype)
        {
            var v = stringValue.TrimEnd('n').Replace("_", "");
            if (!BigInteger.TryParse(v, out var n))
                throw JSContext.Current.NewTypeError($"{stringValue} is not a valid big integer");
            this.value = n;
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

        [JSExport("toString")]
        public JSValue JSToString()
        {
            return new JSString(value.ToString());
        }

        [JSExport("toLocaleString")]
        public JSValue ToLocaleString(in Arguments a)
        {
            return new JSString(value.ToString(CultureInfo.CurrentCulture));
        }


        [JSExport("valueOf")]
        public override JSValue ValueOf()
        {
            return this;
        }



        [JSExport("asIntN")]
        public static JSValue AsIntN(long bits, JSBigInt bigint)
        {
            if (bits < 0 || bits > 9007199254740991)
            {
                throw JSContext.Current.NewRangeError("Invalid range for bits");
            }
            var n = bigint.value;
            var buffer = n.ToByteArray();
            if (buffer.Length * 8 < bits)
            {
                return bigint;
            }

            var reminderBits = (long)bits % (long)8;

            var length = (int)((long)bits / (long)8);
            if (reminderBits > 0)
            {
                length++;
            }

            var copy = new byte[length];
            Buffer.BlockCopy(buffer, 0, copy, 0, length);

            if (reminderBits > 0)
            {
                // here we need to pad leftmost bits as 1s
                // as BigInteger uses bytes and only if the
                // eighth bit is 1, it will consider it as a
                // negative integer

                // so we need to create mask to first remove
                // bits as byte contains eight bits

                // then check the most significant digit
                // if it is negative, then we need to pad
                // 1s before it

                ref byte last = ref copy[copy.Length - 1];

                byte padMask = 0xFF;

                byte mask = 1;
                byte start = 1;
                reminderBits--;
                while (reminderBits > 0)
                {
                    padMask &= (byte)~start;
                    start <<= 1;
                    start |= 1;
                    mask <<= 1;
                    reminderBits--;
                }
                last &= start;
                var lastValue = last;

                if ((mask & lastValue) > 0)
                {
                    last |= padMask;
                }
            }

            var r = new BigInteger(copy);
            return new JSBigInt(r);

        }


        [JSExport("asUintN")]
        public static JSValue AsUintN(long bits, JSBigInt bigint)
        {
            if (bits < 0 || bits > 9007199254740991)
            {
                throw JSContext.Current.NewRangeError("Invalid range for bits");
            }
            var n = bigint.value;
            if (n.Sign == BigInteger.MinusOne.Sign)
            {
                n = -n;
            }
            var buffer = n.ToByteArray();
            if (buffer.Length * 8 < bits)
            {
                return bigint;
            }

            var reminderBits = (long)bits % (long)8;

            var length = (int)((long)bits / (long)8);
            if (reminderBits > 0)
            {
                length++;
            }

            // extra pad will result in a UInt
            var copy = new byte[length + 1];
            Buffer.BlockCopy(buffer, 0, copy, 0, length);

            if (reminderBits > 0)
            {
                ref byte last = ref copy[length - 1];
                byte start = 1;
                reminderBits--;
                while (reminderBits > 0)
                {
                    start <<= 1;
                    start |= 1;
                    reminderBits--;
                }
                last &= start;
            }

            var r = new BigInteger(copy);
            return new JSBigInt(r);

        }
    }
}
