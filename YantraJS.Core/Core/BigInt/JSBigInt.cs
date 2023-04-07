using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Core.BigInt
{
    // [JSRuntime(typeof(JSBigIntStatic), typeof(JSBigIntPrototype))]
    [JSBaseClass("Object")]
    [JSFunctionGenerator("BigInt")]
    public partial class JSBigInt : JSPrimitive
    {

        internal readonly long value;

        public override bool BooleanValue => value != 0;

        public override double DoubleValue => value;

        public override long BigIntValue => value; 

        public JSBigInt(in Arguments a): base(a.NewPrototype)
        {
            var f = a[0];
            switch (f)
            {
                case JSNumber number:
                    this.value = (long)number.value;
                    return;
                case JSBigInt bigint:
                    value = bigint.value;
                    return;
            }
            if (long.TryParse(f.ToString(), out var n))
            {
                this.value = n;
                return;
            }
            throw JSContext.Current.NewTypeError($"{f} is not a valid big integer");
        }

        public JSBigInt(long value)
        {
            this.value = value;
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

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException();
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
            if (type  == typeof(object))
            {
                value = this.value;
                return true;
            }
            return base.ConvertTo(type, out value);
        }

        [JSExport("toString")]
        public new JSValue ToString()
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
    }
}
