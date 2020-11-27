using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YantraJS.Core.Generator;
using YantraJS.Utils;

namespace YantraJS.Core.Clr
{
    public class ClrProxy : JSObject
    {

        internal readonly object value;
        public ClrProxy(object value)
        {
            this.value = value;
            this.prototypeChain = ClrType.From(value.GetType()).prototype;
        }

        internal ClrProxy(object value, JSObject prototypeChain)
        {
            this.value = value;
            this.prototypeChain = prototypeChain;
        }


        public override bool BooleanValue => this.value is bool bv 
            ? bv
            : this.DoubleValue != 0;

        /// <summary>
        /// Todo improvise...
        /// </summary>
        public override double DoubleValue { 
            get
            {
                switch(value)
                {
                    case double @double:
                        return @double;
                    case decimal @decimal:
                        return (double)@decimal;
                    case float @float:
                        return @float;
                    case int @int:
                        return @int;
                    case uint @int:
                        return @int;
                    case long @long:
                        return @long;
                    case ulong @ulong:
                        return @ulong;
                    case short @short:
                        return @short;
                    case ushort @ushort:
                        return @ushort;
                    case byte @byte:
                        return @byte;
                    case sbyte @sbyte:
                        return @sbyte;
                }
                // coerce to double...
                return NumberParser.CoerceToNumber(value.ToString());
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public override bool ConvertTo(Type type, out object value)
        {
            if (type.IsAssignableFrom(this.value.GetType()))
            {
                value = this.value;
                return true;
            }
            return base.ConvertTo(type, out value);
        }

        /// <summary>
        /// Can be improved in future !!
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JSValue Marshal(object value)
        {
            switch (value)
            {
                case null:
                    return JSNull.Value;
                case JSValue jsValue:
                    return jsValue;
                case string @string:
                    return new JSString(@string);
                case int @int:
                    return new JSNumber(@int);
                case uint @uint:
                    return new JSNumber(@uint);
                case long @long:
                    return new JSNumber(@long);
                case ulong @ulong:
                    return new JSNumber(@ulong);
                case double @double:
                    return new JSNumber(@double);
                case float @float:
                    return new JSNumber(@float);
                case bool @bool:
                    return @bool ? JSBoolean.True : JSBoolean.False;
                case short @short:
                    return new JSNumber(@short);
                case byte @byte:
                    return new JSNumber(@byte);
                case sbyte @sbyte:
                    return new JSNumber(@sbyte);
                case DateTime dateTime:
                    return new JSDate(dateTime.ToLocalTime());
                case DateTimeOffset dateTimeOffset:
                    return new JSDate(dateTimeOffset);
                case Type type:
                    return ClrType.From(type);
                case Task<JSValue> task:
                    return task.ToPromise();
                case Task task:
                    return task.ToPromise();
                case IEnumerable<JSValue> en:
                    return new JSGenerator(new ClrEnumerableElementEnumerator(en), "Clr Iterator");
            }

            return new ClrProxy(value);
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBoolean.True;
            if (value is  ClrProxy proxy)
            {
                if (this.value == proxy.value)
                    return JSBoolean.True;
                if (this.value.Equals(proxy.value))
                    return JSBoolean.True;
                // convert to string to compare...
                if (this.value.ToString() == proxy.value.ToString())
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBoolean.True;
            switch(value)
            {
                case ClrProxy proxy:
                    if (this.value == proxy.value)
                        return JSBoolean.True;
                    if (this.value.Equals(proxy.value))
                        return JSBoolean.True;
                    // convert to string to compare...
                    if (this.value.ToString() == proxy.value.ToString())
                        return JSBoolean.True;
                    break;
                case JSString @string when this.value.ToString() == @string.value:
                    return JSBoolean.True;
                case JSNumber number:
                    switch (this.value)
                    {
                        case int @int when @int == (int)number.value:
                            return JSBoolean.True;
                        case uint @uint when @uint == (uint)number.value:
                            return JSBoolean.True;
                        case long @long when @long == (long)number.value:
                            return JSBoolean.True;
                        case ulong @ulong when @ulong == (ulong)number.value:
                            return JSBoolean.True;
                        case double @double when @double == number.value:
                            return JSBoolean.True;
                        case float @float when @float == (float)number.value:
                            return JSBoolean.True;
                    }
                    break;
            }

            // in case left side is not ClrProxy but maybe a string/number/bool/bigint


            return JSBoolean.False;
        }

        public override JSValue this[uint name]
        {
            get
            {
                return (prototypeChain as ClrType.ClrPrototype).GetElementAt(this.value, name);
            }
            set
            {
                try
                {
                    var cp = prototypeChain as ClrType.ClrPrototype;
                    cp.SetElementAt(this.value, name, value);
                } catch (Exception ex)
                {
                    throw new JSException(ex.Message);
                }
            }
        }


        internal override IElementEnumerator GetElementEnumerator()
        {
            if (value is IEnumerable en) {
                return new EnumerableElementEnumerable(en.GetEnumerator());
            }
            throw JSContext.Current.NewTypeError($"{this} is not an iterable");
        }

    }
}
