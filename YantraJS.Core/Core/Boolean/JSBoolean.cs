using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Schema;

namespace YantraJS.Core
{
    public partial class JSBoolean : JSPrimitive
    {

        public static JSBoolean True = new JSBoolean(true);

        public static JSBoolean False = new JSBoolean(false);

        internal readonly bool _value;

        private JSBoolean(bool _value) : base()
        {
            this._value = _value;
        }

        protected override JSObject GetPrototype()
        {
            return JSContext.Current.BooleanPrototype;
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            var first = a.Get1();
            return first.BooleanValue ? JSBoolean.True : JSBoolean.False;
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

        public override bool ConvertTo(Type type, out object value)
        {
            if (type == typeof(bool))
            {
                value = this._value;
                return true;
            }
            if (type.IsAssignableFrom(typeof(JSBoolean)))
            {
                value = this;
                return true;
            }
            if (type == typeof(object))
            {
                value = this._value;
                return true;
            }

            value = null;
            return false;
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
                return JSBoolean.True;
            if (!this._value)
            {
                if (value.IsNullOrUndefined)
                    return JSBoolean.False;
            }
            if (this._value) {
                if (value.DoubleValue == 1)
                    return JSBoolean.True;
            } else
            {
                if (value.DoubleValue == 0)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (value.IsBoolean && value.BooleanValue == this._value)
                return JSBoolean.True;
            return JSBoolean.False; 
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException("boolean is not a function");
        }

        internal override PropertyKey ToKey(bool create = false)
        {
            return this._value ? KeyStrings.@true : KeyStrings.@false;
        }
    }
}
