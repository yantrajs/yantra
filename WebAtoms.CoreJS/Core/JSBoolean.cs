using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Schema;

namespace WebAtoms.CoreJS.Core
{
    public class JSBoolean : JSValue
    {
        internal readonly bool _value;

        internal JSBoolean(bool _value, JSValue prototype) : base(prototype)
        {
            this._value = _value;
        }

        public static bool IsTrue(JSValue value)
        {
            switch (value)
            {
                case JSString str:
                    return str.Length > 0;
                case JSBoolean bv:
                    return bv._value;
                case JSNumber n:
                    return n.value != 0 && n.value != double.NaN;
            }
            return false;
        }

        public override double DoubleValue => this._value ? 1 : 0;

        public override string ToString()
        {
            return _value ? "true" : "false";
        }

        internal static JSFunction Create()
        {
            var r = new JSFunction((t, a) => IsTrue(a[0]) ? JSContext.Current.True : JSContext.Current.False);
            return r;
        }

        public override bool Equals(object obj)
        {
            if (obj is JSBoolean b)
                return this._value == b._value;
            return base.Equals(obj);
        }

        public override JSValue Add(JSValue value)
        {
            switch (value)
            {
                case JSUndefined u:
                    return JSContext.Current.NaN;
                case JSNull n:
                    return this._value ? JSContext.Current.One : JSContext.Current.Zero;
                case JSNumber n1:
                    var v = n1.value;
                    if (double.IsNaN(v)
                        || double.IsPositiveInfinity(v)
                        || double.IsNegativeInfinity(v))
                    {
                        return n1;
                    }
                    return this.Add(v);
            }
            return new JSString(this._value.ToString() + value.ToString());
        }

        public override JSValue Add(double value)
        {
            var v = this._value ? 1 : 0;
            return new JSNumber(v + value);
        }

        public override JSValue Add(string value)
        {
            return new JSString((this._value ? "true" : "false") + value);
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (value is JSBoolean b && b._value == _value)
                return JSContext.Current.True;
            var v = value.DoubleValue;
            if (
                this._value && v == 1
                ||
                !this._value && v == 0)
            {
                return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (value is JSBoolean b && b._value == _value)
                return JSContext.Current.True;
            return JSContext.Current.False; 
        }
    }
}
