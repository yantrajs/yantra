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

        public override JSValue TypeOf()
        {
            return JSConstants.Boolean;
        }

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

        public override JSBoolean Equals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            if (this._value) {
                if (value.DoubleValue == 1)
                    return JSContext.Current.True;
            } else
            {
                if (value.DoubleValue == 0)
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (value.IsBoolean && value.BooleanValue == this._value)
                return JSContext.Current.True;
            return JSContext.Current.False; 
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw new NotImplementedException("boolean is not a function");
        }

        private static KeyString @true = KeyStrings.GetOrCreate("true");
        private static KeyString @false = KeyStrings.GetOrCreate("false");

        internal override KeyString ToKey()
        {
            return this._value ? @true : @false;
        }
    }
}
