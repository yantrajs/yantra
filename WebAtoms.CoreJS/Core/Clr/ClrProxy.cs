using System;
using System.Linq;

namespace WebAtoms.CoreJS.Core.Clr
{
    public class ClrProxy : JSObject
    {

        object value;
        public ClrProxy(object value)
        {
            this.value = value;
            this.prototypeChain = ClrType.From(value.GetType()).prototype;
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
            value = null;
            return false;
        }

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

            }
            return new ClrProxy(value);
        }

    }
}
