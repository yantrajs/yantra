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
using YantraJS.Core.Typed;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{
    // [JSRuntime(typeof(JSStringStatic), typeof(JSStringPrototype))]
    [JSBaseClass("Object")]
    [JSFunctionGenerator("String")]
    public partial class JSString : JSPrimitive
    {

        internal static JSString Empty = new JSString(string.Empty);

        internal readonly string value;
        KeyString _keyString;

        private double NumberValue = 0;
        private bool NumberParsed = false;

        public override double DoubleValue
        {
            get
            {
                if (NumberParsed)
                    return NumberValue;
                NumberValue = NumberParser.CoerceToNumber(value);
                NumberParsed = true;
                return NumberValue;
            }
        }

        

        public override bool BooleanValue => value.Length > 0;

        public override long BigIntValue => long.TryParse(this.ToString(), out var n) ? n : 0;

        public override bool IsString => true;

        public override JSValue AddValue(double value)
        {
            if (this.value.IsEmpty())
                return new JSString(value.ToString());
            return new JSString( string.Concat(this.value, value.ToString()) );
        }

        public override JSValue AddValue(string value)
        {
            if (this.value.IsEmpty())
                return new JSString(value);
            return new JSString( string.Concat(this.value, value));
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
                return new JSString(string.Concat(this.value, vString.value));
            }

            if (value.IsObject)
            {
                value = value.ValueOf();
            }

            if (this.value.IsEmpty())
                return new JSString(value.StringValue);

            var v = value.StringValue;
            if (v.Length == 0)
            {
                return this;
            }
            return new JSString(string.Concat(this.value, v));
        }

        public override bool ConvertTo(Type type, out object value)
        {
            if (type == typeof(string))
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
            if (_keyString.HasValue)
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
                if(!KeyStrings.TryGet(this.value, out _keyString))
                    return KeyStrings.undefined;
                return _keyString;
            }
            return _keyString.Value != null
                ? _keyString
                : (_keyString = KeyStrings.GetOrCreate(this.value));
        }

        protected override JSObject GetPrototype()
        {
            return (JSContext.Current[Names.String] as JSFunction).prototype;
        }

        public JSString(string value): base()
        {
            this.value = value;
        }
        public JSString(JSObject prototype, string value): base(prototype)
        {
            this.value = value;
        }

        public JSString(in StringSpan value) : base()
        {
            this.value = value.Value;
        }


        public JSString(char ch) : this(new string(ch,1))
        {
            
        }


        public JSString(in StringSpan value, KeyString keyString) : this(value)
        {
            this._keyString = keyString;
        }

        public static implicit operator KeyString(JSString value)
        {
            return value.ToString();
        }

        public override JSValue TypeOf()
        {
            return JSConstants.String;
        }


        public override string ToString()
        {
            return value;
        }

        public byte[] Encode(System.Text.Encoding encoding)
        {
            return encoding.GetBytes(value);
        }

        public override string ToDetailString()
        {
            return value;
        }

        public override string ToLocaleString(string format, CultureInfo culture)
        {

            return value;
          
        }

        internal protected override JSValue GetValue(uint key, JSValue receiver, bool throwError = true)
        {
            if (key >= this.value.Length)
            {
                return JSUndefined.Value;
            }
            return new JSString(new string(this.value[(int)key], 1));
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

        public override bool Less(JSValue value)
        {
            if (value.IsUndefined)
                return false;
            if (value.CanBeNumber)
            {
                return this.DoubleValue < value.DoubleValue;
            }
            return this.value.Less(value.ToString());

        }

        public override bool LessOrEqual(JSValue value)
        {
            if (value.IsUndefined)
                return false;
            if (value.CanBeNumber)
            {
                return (this.DoubleValue <= value.DoubleValue);
            }
            return this.value.LessOrEqual(value.ToString());
        }

        public override bool Greater(JSValue value)
        {
            if (value.IsUndefined)
                return false;
            if (value.CanBeNumber)
            {
                return (this.DoubleValue > value.DoubleValue);
            }
            return this.value.Greater(value.ToString());
        }

        public override bool GreaterOrEqual(JSValue value)
        {
            if (value.IsUndefined)
                return false;
            if (value.CanBeNumber)
            {
                return (this.DoubleValue >= value.DoubleValue);
            }
            return this.value.GreaterOrEqual(value.ToString());
        }

        public override bool StrictEquals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return true;
            if (value is JSString s)
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
            private StringSpan.CharEnumerator en;
            int index;
            public ElementEnumerator(in StringSpan value)
            {
                this.en = value.GetEnumerator();
                index = -1;
            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint i)
            {
                if (en.MoveNext(out var ch))
                {
                    index++;
                    i = (uint)index;
                    hasValue = true;
                    value = new JSString(new string(ch, 1));
                    return true;
                }
                i = 0;
                value = JSUndefined.Value;
                hasValue = false;
                return false;
            }

            public bool MoveNext(out JSValue value)
            {
                if (en.MoveNext(out var ch))
                {
                    index++;
                    value = new JSString(new string(ch, 1));
                    return true;
                }
                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNextOrDefault(out JSValue value, JSValue @default)
            {
                if (en.MoveNext(out var ch))
                {
                    index++;
                    value = new JSString(new string(ch, 1));
                    return true;
                }
                value = @default;
                return false;
            }

            public JSValue NextOrDefault(JSValue @default)
            {
                if (en.MoveNext(out var ch))
                {
                    index++;
                    return new JSString(new string(ch, 1));
                }
                return @default;
            }

        }

    }
}
