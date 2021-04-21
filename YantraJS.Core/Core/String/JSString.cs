using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using YantraJS.Core.Typed;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{
    [JSRuntime(typeof(JSStringStatic), typeof(JSStringPrototype))]
    public partial class JSString : JSPrimitive
    {

        internal static JSString Empty = new JSString(string.Empty);

        internal readonly StringSpan value;
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

        public override long BigIntValue => long.TryParse(value.Value, out var n) ? n : 0;

        public override bool IsString => true;

        public override bool ConvertTo(Type type, out object value)
        {
            if (type == typeof(string))
            {
                value = this.value.Value;
                return true;
            }
            if (type == typeof(object))
            {
                value = this.value.Value;
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

        internal override KeyString ToKey(bool create = true)
        {
            if (_keyString.HasValue)
                return _keyString;
            var d = this.DoubleValue;
            if (!double.IsNaN(d))
            {
                if (d >= 0 && (d % 1 == 0))
                {
                    _keyString = new KeyString((uint)d);
                    return _keyString;
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
            return JSContext.Current.StringPrototype;
        }

        public JSString(string value): base()
        {
            this.value = value;
        }

        public JSString(in StringSpan value) : base()
        {
            this.value = value;
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
            return value.Value;
        }

        public override string ToDetailString()
        {
            return value.Value;
        }

        public override JSValue this[uint key] { 
            get
            {
                if (key >= this.value.Length)
                    return JSUndefined.Value;
                return new JSString(new string(this.value[(int)key],1));
            }
            set { } 
        }

        public override JSValue this[KeyString name] {
            get {
                this.ResolvePrototype();
                var p = prototypeChain.GetInternalProperty(in name);
                if (p.IsEmpty)
                    return JSUndefined.Value;
                return this.GetValue(p);
            }
            set { }
        }

        internal override IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            return new KeyEnumerator(this.Length);
        }

        public override int Length => value.Length;

        public override bool Equals(object obj)
        {
            if (obj is JSString v)
                return this.value == v.value;
            return base.Equals(obj);
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return JSBoolean.True;
            switch (value)
            {
                case JSString strValue:
                    if(this.value == strValue.value)
                        return JSBoolean.True;
                    return JSBoolean.False;
                case JSNumber number
                    when ((this.DoubleValue == number.value)
                        || (this.value.CompareTo(number.value.ToString()) == 0)):
                    return JSBoolean.True;
                case JSBoolean boolean
                    when (this.DoubleValue == (boolean._value ? 1D : 0D)):
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        internal override JSBoolean Less(JSValue value)
        {
            if (value.IsUndefined)
                return JSBoolean.False;
            if (value.CanBeNumber)
            {
                return (this.DoubleValue < value.DoubleValue)
                    ? JSBoolean.True
                    : JSBoolean.False;
            }
            int n = this.value.CompareTo(value.ToString());
            return n < 0
                ? JSBoolean.True
                : JSBoolean.False;

        }

        internal override JSBoolean LessOrEqual(JSValue value)
        {
            if (value.IsUndefined)
                return JSBoolean.False;
            if (value.CanBeNumber)
            {
                return (this.DoubleValue <= value.DoubleValue)
                    ? JSBoolean.True
                    : JSBoolean.False;
            }
            return this.value.CompareTo(value.ToString()) <= 0
                ? JSBoolean.True
                : JSBoolean.False;
        }

        internal override JSBoolean Greater(JSValue value)
        {
            if (value.IsUndefined)
                return JSBoolean.False;
            if (value.CanBeNumber)
            {
                return (this.DoubleValue > value.DoubleValue)
                    ? JSBoolean.True
                    : JSBoolean.False;
            }
            return this.value.CompareTo(value.ToString()) > 0
                ? JSBoolean.True
                : JSBoolean.False;
        }

        internal override JSBoolean GreaterOrEqual(JSValue value)
        {
            if (value.IsUndefined)
                return JSBoolean.False;
            if (value.CanBeNumber)
            {
                return (this.DoubleValue >= value.DoubleValue)
                    ? JSBoolean.True
                    : JSBoolean.False;
            }
            return this.value.CompareTo(value.ToString()) >= 0
                ? JSBoolean.True
                : JSBoolean.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return JSBoolean.True;
            if (value is JSString s)
                if (s.value == this.value)
                    return JSBoolean.True;
            return JSBoolean.False;
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

        internal override IElementEnumerator GetElementEnumerator()
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


        }

    }
}
