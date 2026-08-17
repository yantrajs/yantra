using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.FastParser;
using YantraJS.Core.Typed;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{
    // [JSRuntime(typeof(JSStringStatic), typeof(JSStringPrototype))]
    [JSBaseClass("Object")]
    [JSFunctionGenerator("String")]
    public partial class JSString : JSValue, IJSPrimitive
    {

        internal static JSString Empty = new JSString(string.Empty);

        internal readonly StringOrChar value;
        KeyString _keyString;

        private double NumberValue = 0;
        private bool NumberParsed = false;

        public override double DoubleValue
        {
            get
            {
                if (NumberParsed)
                    return NumberValue;
                NumberValue = NumberParser.CoerceToNumber(value.ToString());
                NumberParsed = true;
                return NumberValue;
            }
        }

        

        public override bool BooleanValue => value.Length > 0;

        public override long BigIntValue => long.TryParse(this.ToString(), out var n) ? n : 0;

        // public override bool IsString => true;

        public override JSValue AddValue(double value)
        {
            if (this.value.IsEmpty())
                return new JSString(value.ToString());
            return new JSString( this.value.Add(value));
        }

        public override JSValue AddValue(string value)
        {
            if (this.value.IsEmpty())
                return new JSString(value);
            return new JSString( this.value.Add(value));
        }

        public override JSValue AddValue(JSValue value)
        {
            if (value is JSString vString)
            {
                if (this.value.IsEmpty())
                {
                    return vString;
                }
                if (vString.value.IsEmpty())
                {
                    return this;
                }
                return new JSString(this.value.Add(vString.value));
            }

            if (value.IsObject)
            {
                value = value.ValueOf();
            }

            if (this.value.IsEmpty())
                return new JSString(value.ToStringOrChar());

            var v = value.ToStringOrChar();
            if (v.Length == 0)
            {
                return this;
            }
            return new JSString(this.value.Add(v));
        }

        public override bool ConvertTo(Type type, out object value)
        {
            if (type == typeof(string))
            {
                value = this.value.ToString();
                return true;
            }
            if (type == typeof(StringOrChar))
            {
                value = this.value;
                return true;
            }
            if (type == typeof(object))
            {
                value = this.value;
                return true;
            }
            if(type == typeof(char))
            {
                value = this.value[0];
                return true;
            }
            if (type.IsAssignableFrom(typeof(JSString)))
            {
                value = this;
                return true;
            }
            value = null;
            return false;
        }

        internal override PropertyKey ToKey(bool create = true)
        {
            if (_keyString > 0)
                return _keyString;
            var d = this.DoubleValue;
            if (!double.IsNaN(d))
            {
                if (d >= 0 && (d % 1 == 0))
                {
                    return (uint)d;
                }
            }
            if (!create)
            {
                if(!KeyStrings.Instance.TryGet(this.value, out _keyString))
                    return KeyString.undefined;
                return _keyString;
            }
            return _keyString > 0
                ? _keyString
                : (_keyString = KeyStrings.Instance.GetOrCreate(this.value));
        }

        //protected override JSObject GetPrototype()
        //{
        //    return (JSContext.Current[KeyString.String] as JSFunction).prototype;
        //}

        private JSPrototype GetPrototype(JSContext context = null)
        {
            return (context ?? JSContext.Current).String_Prototype;
        }

        internal override JSFunctionDelegate GetMethod(in KeyString key)
        {
            return this.GetPrototype(null).GetMethod(in key);
        }

        protected internal override JSValue GetValue(KeyString key, JSValue receiver, bool throwError = true)
        {
            var p = GetPrototype(null).GetInternalProperty(key);
            return (receiver ?? this).GetValue(p);
        }


        public JSString(StringOrChar value) : base(JSValueType.String, JSContext.CurrentContext.String_Prototype)
        {
            this.value = value;
        }


        public JSString(string value): base(JSValueType.String, JSContext.CurrentContext.String_Prototype)
        {
#if DEBUG
            if(value == null) {
               throw new ArgumentNullException(nameof(value));
            }
#endif
            this.value = value.AsStringOrChar();
        }
        //public JSString(JSObject prototype, string value): base(prototype)
        //{
        //    this.value = value;
        //}

        public JSString(in StringSpan value) : base(JSValueType.String, JSContext.CurrentContext.String_Prototype)
        {
#if DEBUG
            if(value == null) {
               throw new ArgumentNullException(nameof(value));
            }
#endif
            this.value = value.Value.AsStringOrChar();
        }


        public JSString(char ch) : this(new StringOrChar(ch))
        {
            
        }


        public JSString(in StringSpan value, KeyString keyString) : this(value)
        {
            this._keyString = keyString;
        }

        //public static implicit operator KeyString(JSString value)
        //{
        //    return value.ToKey().KeyString;
        //}

        // public override JSValue TypeOf()
        // {
        //     return JSConstants.String;
        // }


        public override string ToString()
        {
            return value.ToString();
        }

        public override StringOrChar ToStringOrChar()
        {
            return value;
        }

        public byte[] Encode(System.Text.Encoding encoding)
        {
            return encoding.GetBytes(value.ToString());
        }

        public override string ToDetailString()
        {
            return value.ToString();
        }

        public override string ToLocaleString(string format, CultureInfo culture)
        {

            return value.ToString();
          
        }

        internal protected sealed override JSValue GetValue(uint key, JSValue receiver, bool throwError = true)
        {
            if (key >= this.value.Length)
            {
                return JSUndefined.Value;
            }
            return new JSString(new StringOrChar(this.value[(int)key]));
        }

        //public override JSValue this[uint key] { 
        //    get
        //    {
        //        if (key >= this.value.Length)
        //            return JSUndefined.Value;
        //        return new JSString(new string(this.value[(int)key],1));
        //    }
        //    set { } 
        //}

        //public override JSValue this[KeyString name] {
        //    get {
        //        this.ResolvePrototype();
        //        var p = prototypeChain.GetInternalProperty(in name);
        //        if (p.IsEmpty)
        //            return JSUndefined.Value;
        //        return this.GetValue(p);
        //    }
        //    set { }
        //}

        public override IEnumerable<JSValue> GetForInKeys()
        {
            for (int i = 0; i< this.value.Length;i++)
            {
                yield return JSNumber.From(i);
            }
            foreach(var item in base.GetForInKeys())
            {
                yield return item;
            }
        }
        public override IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            return new KeyEnumerator(this.Length);
        }

        [JSExport]
        public override int Length => value.Length;

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is JSString v)
                return this.value == v.value;
            return base.Equals(obj);
        }

        public override bool Equals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return true;
            switch (value)
            {
                case JSString strValue:
                    if(this.value == strValue.value)
                        return true;
                    return false;
                case JSNumber number
                    when ((this.DoubleValue == number.value)
                        || (this.value.CompareTo(number.value.ToString()) == 0)):
                    return true;
                case JSBoolean boolean
                    when (this.DoubleValue == (boolean._value ? 1D : 0D)):
                    return true;
            }
            return false;
        }

        public override bool EqualsLiteral(double value)
        {
            return this.DoubleValue == value || this.value.CompareTo(value.ToString()) == 0;
        }

        public override bool EqualsLiteral(string value)
        {
            return this.value.Equals(value);
        }

        public override bool StrictEqualsLiteral(string value)
        {
            return this.value.Equals(value);
        }

        public override bool Less(JSValue right)
        {
            // if (right.IsNullOrUndefined) {
            //     return true;
            // }
            if (right.IsUndefined)
            {
                return false;
            }
            if (right.IsObject) {
                right = right.ValueOf();
            }
            return right.CanBeNumber
                ? DoubleValue < right.DoubleValue
                : value.Less(right.ToStringOrChar());
        }

        public override bool LessLiteral(double right)
        {
            return DoubleValue < right;
        }

        public override bool LessLiteral(string right)
        {
            return value.Less(right.AsStringOrChar());
        }

        public override bool LessOrEqual(JSValue right)
        {
            // if (right.IsNullOrUndefined) {
            //     return true;
            // }
            if (right.IsUndefined) {
                return false;
            }
            if (right.IsObject) {
                right = right.ValueOf();
            }
            return right.CanBeNumber
                ? DoubleValue <= right.DoubleValue
                : value.LessOrEqual(right.ToStringOrChar());
        }

        public override bool LessOrEqualLiteral(double right)
        {
            return this.DoubleValue <= right;
        }

        public override bool LessOrEqualLiteral(string right)
        {
            return value.LessOrEqual(right.AsStringOrChar());
        }

        public override bool Greater(JSValue right)
        {
            // if (right.IsNullOrUndefined) {
            //     return false;
            // }
            if (right.IsUndefined) {
                return false;
            }
            if (right.IsObject) {
                right = right.ValueOf();
            }
            return right.CanBeNumber
                ? DoubleValue > right.DoubleValue
                : value.Greater(right.ToStringOrChar());
        }

        public override bool GreaterLiteral(double right)
        {
            return DoubleValue > right;
        }

        public override bool GreaterLiteral(string right)
        {
            return value.Greater(right.AsStringOrChar());
        }


        public override bool GreaterOrEqual(JSValue right)
        {
            if (right.IsUndefined) {
                return false;
            }
            // if (right.IsNullOrUndefined) {
            //     return false;
            // }
            if (right.IsObject) {
                right = right.ValueOf();
            }
            return right.CanBeNumber
                ? DoubleValue >= right.DoubleValue
                : value.GreaterOrEqual(right.ToStringOrChar());
        }

        public override bool GreaterOrEqualLiteral(double right)
        {
            return DoubleValue >= right;
        }

        public override bool GreaterOrEqualLiteral(string right)
        {
            return value.GreaterOrEqual(right.AsStringOrChar());
        }

        public override bool StrictEquals(JSValue right)
        {
            if (object.ReferenceEquals(this, right))
                return true;
            if (right is JSString s)
                if (s.value.Equals(this.value))
                    return true;
            return false;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException($"\"{value}\" is not a function");
        }

        internal override JSBoolean Is(JSValue value)
        {
            if (value is JSString @string && this.value == @string.value)
                return JSBoolean.True;
            return JSBoolean.False;

        }

        public override IElementEnumerator GetElementEnumerator()
        {
            return new ElementEnumerator(this.value);
        }

        private struct ElementEnumerator : IElementEnumerator
        {
            private StringOrChar value;
            int index;
            int last;
            public ElementEnumerator(StringOrChar value)
            {
                this.value = value;
                last = value.Length - 1;
                index = -1;
            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint i)
            {
                if (last > index)
                {
                    index++;
                    i = (uint)index;
                    hasValue = true;
                    value = new JSString(new StringOrChar(this.value[index]));
                    return true;
                }
                i = 0;
                value = JSUndefined.Value;
                hasValue = false;
                return false;
            }

            public bool MoveNext(out JSValue value)
            {
                if (last > index)
                {
                    index++;
                    value = new JSString(new StringOrChar(this.value[index]));
                    return true;
                }
                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNextOrDefault(out JSValue value, JSValue @default)
            {
                if (last > index)
                {
                    index++;
                    value = new JSString(new StringOrChar(this.value[index]));
                    return true;
                }
                value = @default;
                return false;
            }

            public JSValue NextOrDefault(JSValue @default)
            {
                if (last > index)
                {
                    index++;
                    return new JSString(new StringOrChar(this.value[index]));
                }
                return @default;
            }

        }

    }
}
