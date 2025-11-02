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

namespace YantraJS.Core
{
    static class JSDecimalExtensions
    {
        public static decimal AsDecimalOnly(this JSValue @this)
        {
            return @this is JSDecimal v ? v.value : throw JSDecimal.CannotMix();
        }
    }

    // [JSRuntime(typeof(JSBigIntStatic), typeof(JSBigIntPrototype))]
    [JSBaseClass("Object")]
    [JSFunctionGenerator("Decimal")]
    public partial class JSDecimal : JSPrimitive
    {


        public static JSException CannotMix()
        {
            return JSContext.Current.NewTypeError("Cannot mix BigInt and other types, use explicit conversions");
        }

        internal readonly decimal value;

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
                    return new JSDecimal((decimal)number.value);
                case JSDecimal bigint:
                    return bigint;
            }
            var text = f.ToString();
            text = text.TrimEnd('m').Replace("_", "");
            if (!decimal.TryParse(text, out var v))
            {
                throw JSContext.Current.NewTypeError($"{f} is not a valid big integer");
            }
            return new JSDecimal(v);

        }

        public JSDecimal(decimal value)
        {
            this.value = value;
        }
        public JSDecimal(string stringValue)
        {
            var v = stringValue.TrimEnd('m').Replace("_", "");
            if (!decimal.TryParse(v, out var n))
                throw JSContext.Current.NewTypeError($"{stringValue} is not a valid big integer");
            this.value = n;
        }

        public override bool Equals(JSValue value)
        {
            if (value is JSDecimal bigint)
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
                return new JSDecimal(0);
            }
            var value = a[0];
            if (value is JSDecimal d)
            {
                return d;
            }
            if (value.IsNumber)
            {
                return new JSDecimal((long)value.DoubleValue);
            }
            var v = long.Parse(value.ToString());
            return new JSDecimal(v);
        }

        public override bool StrictEquals(JSValue value)
        {
            if (!(value is JSDecimal bigint))
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
            return JSConstants.Decimal;
        }

        protected override JSObject GetPrototype()
        {
            return (JSContext.Current[Names.Decimal] as JSFunction).prototype;
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
            if (type.IsAssignableFrom(typeof(JSDecimal)))
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
            return new JSDecimal(-this.value);
        }

        public override JSValue BitwiseAnd(JSValue value)
        {
            throw JSDecimal.CannotMix();
        }

        public override JSValue BitwiseOr(JSValue value)
        {
            throw JSDecimal.CannotMix();
        }

        public override JSValue BitwiseXor(JSValue value)
        {
            throw JSDecimal.CannotMix();
        }

        public override JSValue LeftShift(JSValue value)
        {
            throw JSDecimal.CannotMix();
        }

        public override JSValue RightShift(JSValue value)
        {
            throw JSDecimal.CannotMix();
        }

        public override JSValue UnsignedRightShift(JSValue value)
        {
            throw JSDecimal.CannotMix();
        }

        public override JSValue Multiply(JSValue value)
        {
            return new JSDecimal(this.value * value.AsDecimalOnly());
        }

        public override JSValue Divide(JSValue value)
        {
            return new JSDecimal(this.value / value.AsDecimalOnly());
        }

        public override JSValue Subtract(JSValue value)
        {
            return new JSDecimal(this.value - value.AsDecimalOnly());
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
            if (value is JSDecimal b)
            {
                return new JSDecimal(this.value + b.value);
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
            return new JSDecimal(this.value + value.BigIntValue);
        }

        [JSExport("toString")]
        public JSValue JSToString()
        {
            return new JSString(value.ToString());
        }


        [JSExport("toFixed")]
        public JSValue JSToFixed(in Arguments a)
        {
            var nv = value;
            if (a.Get1() is JSNumber n1)
            {
                if (double.IsNaN(n1.value) || n1.value > 20 || n1.value < 0)
                    throw JSContext.Current.NewRangeError("toFixed() digitis argument must be between 0 and 100");
                var i = (int)n1.value;
                if (nv > 999999999999999.0m && i <= 15)
                    return new JSString(nv.ToString("g21"));
                return new JSString(nv.ToString($"F{i}"));
            }
            if (nv > 999999999999999.0m)
                return new JSString(nv.ToString("g21"));
            return new JSString(nv.ToString("F0"));
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


    }
}
