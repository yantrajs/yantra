﻿using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Schema;

namespace WebAtoms.CoreJS.Core
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
                return JSBoolean.True;
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

        private static KeyString @true = KeyStrings.GetOrCreate("true");
        private static KeyString @false = KeyStrings.GetOrCreate("false");

        internal override KeyString ToKey(bool create = false)
        {
            return this._value ? @true : @false;
        }
    }
}