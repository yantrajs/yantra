using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Schema;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSBoolean : JSValue
    {
        internal readonly bool _value;

        internal JSBoolean(bool _value, JSObject prototype) : base(prototype)
        {
            this._value = _value;
        }

        //public static bool IsTrue(JSValue value)
        //{
        //    switch (value)
        //    {
        //        case JSString str:
        //            return str.Length > 0;
        //        case JSBoolean bv:
        //            return bv._value;
        //        case JSNumber n:
        //            return n.value != 0 && n.value != double.NaN;
        //        case JSObject obj:
        //            return true;
        //    }
        //    return false;
        //}

        public override double DoubleValue => this._value ? 1 : 0;

        public override bool BooleanValue => this._value;

        public override bool IsBoolean => true; 

        public override string ToString()
        {
            return _value ? "true" : "false";
        }

        public override bool Equals(object obj)
        {
            if (obj is JSBoolean b)
                return this._value == b._value;
            return base.Equals(obj);
        }

        public override JSValue AddValue(JSValue value)
        {
            if (value.IsUndefined)
                return JSContext.Current.NaN;
            if (value.IsNull)
                return this._value ? JSContext.Current.One : JSContext.Current.Zero;
            if (value.IsNumber)
                return new JSNumber(value.DoubleValue + (this._value ? 1 : 0));
            return new JSString(this._value.ToString() + value.ToString());
        }

        public override JSValue AddValue(double value)
        {
            var v = this._value ? 1 : 0;
            return new JSNumber(v + value);
        }

        public override JSValue AddValue(string value)
        {
            return new JSString((this._value ? "true" : "false") + value);
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            switch (value)
            {
                case JSBoolean boolean
                    when this._value == boolean._value:
                    return JSContext.Current.True;
                case JSNumber number
                    when (1D == number.value):
                    return JSContext.Current.True;
                case JSString @string
                    when (1D == @string.DoubleValue):
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

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw new NotImplementedException("boolean is not a function");
        }

        internal override JSBoolean Less(JSValue value)
        {
            switch(value)
            {
                case JSBoolean boolean 
                    when (!this._value && boolean._value):
                        return JSContext.Current.True;
                case JSNumber number 
                    when (1D < number.value):
                        return JSContext.Current.True;
                case JSString @string 
                    when (1D < @string.DoubleValue):
                        return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean LessOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            switch (value)
            {
                case JSBoolean boolean 
                    when ((!this._value && boolean._value) || this._value == boolean._value):
                        return JSContext.Current.True;
                case JSNumber number 
                    when (1D <= number.value):
                        return JSContext.Current.True;
                case JSString @string 
                    when (1D <= @string.DoubleValue):
                        return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean Greater(JSValue value)
        {
            switch (value)
            {
                case JSBoolean boolean
                    when (this._value && !boolean._value):
                    return JSContext.Current.True;
                case JSNumber number
                    when (1D > number.value):
                    return JSContext.Current.True;
                case JSString @string
                    when (1D > @string.DoubleValue):
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean GreaterOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            switch (value)
            {
                case JSBoolean boolean
                    when ((this._value && !boolean._value) || (this._value == boolean._value)):
                    return JSContext.Current.True;
                case JSNumber number
                    when (1D >= number.value):
                    return JSContext.Current.True;
                case JSString @string
                    when (1D >= @string.DoubleValue):
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        private static KeyString @true = KeyStrings.GetOrCreate("true");
        private static KeyString @false = KeyStrings.GetOrCreate("false");

        internal override KeyString ToKey()
        {
            return this._value ? @true : @false;
        }
    }
}
