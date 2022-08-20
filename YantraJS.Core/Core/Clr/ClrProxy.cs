using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YantraJS.Core.Generator;
using YantraJS.Utils;

namespace YantraJS.Core.Clr
{
    public partial class ClrProxy : JSObject
    {

        internal readonly object value;
        public ClrProxy(object value)
        {
            this.value = value;
            this.BasePrototypeObject = ClrType.From(value.GetType()).prototype;
            AddExtensionMethods();
        }

        

        internal ClrProxy(object value, JSObject prototypeChain)
        {
            this.value = value;
            this.BasePrototypeObject = prototypeChain;
            AddExtensionMethods();
        }
        private void AddExtensionMethods()
        {
            var extensionMethods = ExtensionMethodHelper.GetExtensionMethods(this.value.GetType());

            if (extensionMethods != null)
            {
                foreach (var method in extensionMethods)
                {
                    if (ExtensionMethodHelper.IsJsFunctionDelegate(method))
                    {
                        this[method.Name.ToCamelCase()] =
                            ExtensionMethodHelper.ExtensionMethodToFunction(method.DeclaringType, method, this.value,
                                true);
                    }
                    else
                    {
                        this[method.Name.ToCamelCase()] =
                            ExtensionMethodHelper.ExtensionMethodToFunction(method.DeclaringType, method, this.value,
                                false);
                    }
                }

            }
        }


        public override bool BooleanValue
        {
            get {
                if (value == null)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Todo improvise...
        /// </summary>
        public override double DoubleValue { 
            get
            {
                if (value == null)
                    return 0;
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

        internal static MethodInfo[] methods = typeof(ClrProxy)
            .GetMethods()
            .Where(x => x.Name == "Marshal" 
                && x.IsPublic 
                && x.ReturnType == typeof(JSValue)
                && x.GetParameters().Length == 1
                && x.GetParameters()[0].ParameterType != typeof(object)).ToArray();
        public static Func<TInput,JSValue> GetDelegate<TInput>()
        {
            var method = methods.FirstOrDefault(x => x.GetParameters()[0].ParameterType == typeof(TInput));
            if (method != null)
            {
                return (Func<TInput, JSValue>)method.CreateDelegate(typeof(Func<TInput, JSValue>));
            }
            return (input) => ClrProxy.Marshal((object)input);
        }

        public static JSValue Marshal(int value) => new JSNumber(value);
        public static JSValue Marshal(uint value) => new JSNumber(value);
        public static JSValue Marshal(long value) => new JSNumber(value);
        public static JSValue Marshal(ulong value) => new JSNumber(value);

        public static JSValue Marshal(string value) => new JSString(value);

        public static JSValue Marshal(in StringSpan value) => new JSString(value);

        public static JSValue Marshal(bool value) => value ? JSBoolean.True : JSBoolean.False;

        public static JSValue Marshal(short value) => new JSNumber(value);

        public static JSValue Marshal(ushort value) => new JSNumber(value);

        public static JSValue Marshal(byte value) => new JSNumber(value);

        public static JSValue Marshal(sbyte value) => new JSNumber(value);

        public static JSValue Marshal(DateTime value) => new JSDate(value);

        public static JSValue Marshal(DateTimeOffset value) => new JSDate(value);

        public static JSValue Marshal(double value) => new JSNumber(value);

        public static JSValue Marshal(float value) => new JSNumber(value);

        public static JSValue Marshal(Task task) => task.ToPromise();

        public static JSValue Marshal(IEnumerable<JSValue> en) => new JSGenerator(new ClrEnumerableElementEnumerator(en), "Clr Iterator");

        /// <summary>
        /// Can be improved in future !!
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JSValue Marshal(object value)
        {
            if (value == null)
                return JSNull.Value;
            var type = value.GetType();
            if (type.IsEnum)
                return new JSString(value.ToString());
            var t = Type.GetTypeCode(type);
            switch (t)
            {
                case TypeCode.Boolean:
                    return (bool)value ? JSBoolean.True : JSBoolean.False;
                case TypeCode.Byte:
                    return new JSNumber((byte)value);
                case TypeCode.Char:
                    return new JSString((char)value);
                case TypeCode.DateTime:
                    return new JSDate((DateTime)value);
                case TypeCode.DBNull:
                    return JSNull.Value;
                case TypeCode.Decimal:
                    return new JSNumber((double)(decimal)value);
                case TypeCode.Double:
                    return new JSNumber((double)value);
                case TypeCode.Int16:
                    return new JSNumber((short)value);
                case TypeCode.Int32:
                    return new JSNumber((int)value);
                case TypeCode.Int64:
                    return new JSNumber((long)value);
                case TypeCode.SByte:
                    return new JSNumber((sbyte)value);
                case TypeCode.Single:
                    return new JSNumber((float)value);
                case TypeCode.String:
                    return new JSString((string)value);
                case TypeCode.UInt16:
                    return new JSNumber((ushort)value);
                case TypeCode.UInt32:
                    return new JSNumber((uint)value);
                case TypeCode.UInt64:
                    return new JSNumber((long)value);
            }

            switch (value)
            {
                case JSValue jsValue:
                    return jsValue;
                case DateTimeOffset dateTimeOffset:
                    return new JSDate(dateTimeOffset);
                case Type valueType:
                    return ClrType.From(valueType);
                case Task<JSValue> task:
                    return task.ToPromise();
                case Task task:
                    return task.ToPromise();
                case IEnumerable<JSValue> en:
                    return new JSGenerator(new ClrEnumerableElementEnumerator(en), "Clr Iterator");
            }

            return new ClrProxy(value);
        }

        public override bool Equals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return true;
            if (value is  ClrProxy proxy)
            {
                if (this.value == proxy.value)
                    return true;
                if (this.value.Equals(proxy.value))
                    return true;
                // convert to string to compare...
                if (this.value.ToString() == proxy.value.ToString())
                    return true;
            }
            return false;
        }

        public override bool StrictEquals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return true;
            switch(value)
            {
                case ClrProxy proxy:
                    if (this.value == proxy.value)
                        return true;
                    if (this.value.Equals(proxy.value))
                        return true;
                    // convert to string to compare...
                    if (this.value.ToString() == proxy.value.ToString())
                        return true;
                    break;
                case JSString @string when this.value.ToString() == @string.value:
                    return true;
                case JSNumber number:
                    switch (this.value)
                    {
                        case int @int when @int == (int)number.value:
                            return true;
                        case uint @uint when @uint == (uint)number.value:
                            return true;
                        case long @long when @long == (long)number.value:
                            return true;
                        case ulong @ulong when @ulong == (ulong)number.value:
                            return true;
                        case double @double when @double == number.value:
                            return true;
                        case float @float when @float == (float)number.value:
                            return true;
                    }
                    break;
            }

            // in case left side is not ClrProxy but maybe a string/number/bool/bigint


            return false;
        }

        public override JSValue this[uint name]
        {
            get
            {
                return (prototypeChain?.@object as ClrType.ClrPrototype).GetElementAt(this.value, name);
            }
            set
            {
                try
                {
                    var cp = prototypeChain?.@object as ClrType.ClrPrototype;
                    cp.SetElementAt(this.value, name, value);
                } catch (Exception ex)
                {
                    throw new JSException(ex.Message);
                }
            }
        }


        public override IElementEnumerator GetElementEnumerator()
        {
            if (value is IEnumerable en) {
                return new EnumerableElementEnumerable(en.GetEnumerator());
            }
            throw JSContext.Current.NewTypeError($"{this} is not an iterable");
        }

    }
}
