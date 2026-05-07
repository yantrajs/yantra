using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Schema;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Primitive;

namespace YantraJS.Core
{
    internal static class JSBooleanStatic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSBoolean AsBooleanOnly(this JSValue value, [CallerMemberName] string name = null)
        {
            if (value is JSPrimitiveObject p)
            {
                value = p.value;
            }
            if (!(value is JSBoolean b))
            {
                throw JSContext.Current.NewTypeError($"Boolean.prototype.{name} requires that 'this' be a Number");
            }
            return b;
        }
    }



    [JSBaseClass("Object")]
    [JSFunctionGenerator("Boolean")]
    public partial class JSBoolean : JSPrimitive
    {


        public static JSBoolean True = new JSBoolean(true);

        public static JSBoolean False = new JSBoolean(false);

        internal readonly bool _value;

        private JSBoolean(bool _value) : base(JSValueType.Boolean, _value)
        {
            this._value = _value;
        }

        [JSExport(IsConstructor = true)]
        public static JSValue Constructor(in Arguments a)
        {
            return new JSPrimitiveObject( (a[0]?.BooleanValue ?? false) ? True : False);
        }

        protected override JSObject GetPrototype()
        {
            return (JSContext.Current[KeyString.Boolean] as JSFunction).prototype; ;
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

        // public override bool BooleanValue => this._value;

        // public override bool IsBoolean => true;

        // public override JSValue TypeOf()
        // {
        //     return JSConstants.Boolean;
        // }

        public override JSValue Negate()
        {
            return this._value ? JSNumber.MinusOne : JSNumber.NegativeZero;
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

        public override int GetHashCode()
        {
            return _value ? 1 : 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is JSBoolean b)
                return this._value == b._value;
            return base.Equals(obj);
        }

        public override bool Equals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return true;
            if (!this._value)
            {
                if (value.IsNullOrUndefined)
                    return false;
            }
            if (this._value) {
                if (value.DoubleValue == 1)
                    return true;
            } else
            {
                if (value.DoubleValue == 0)
                    return true;
            }
            return false;
        }

        public override bool EqualsLiteral(double value)
        {
            return this._value 
                ? value == 1
                : value == 0;
        }

        public override bool EqualsLiteral(string value)
        {
            return this._value 
                ? value == "1"
                : value == "0";
        }

        public override bool StrictEquals(JSValue value)
        {
            return Object.ReferenceEquals(this, value);
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException("boolean is not a function");
        }

        internal override PropertyKey ToKey(bool create = false)
        {
            return this._value ? KeyString.@true : KeyString.@false;
        }

        [JSPrototypeMethod]
        [JSExport("toString", Length = 1)]
        public static JSString ToString(in Arguments a)
        {
            return new JSString(a.This.AsBooleanOnly()._value ? "true" : "false");
        }

        [JSPrototypeMethod]
        [JSExport("valueOf")]
        public static JSValue ValueOf(in Arguments a)
        {
            return a.This.AsBooleanOnly();
        }

        [JSPrototypeMethod]
        [JSExport("toLocaleString", Length = 1)]

        public static JSString ToLocaleString(in Arguments a)
        {
            return new JSString(a.This.AsBooleanOnly()._value ? "true" : "false");
        }
    }
}
