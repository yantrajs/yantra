using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSString : JSValue
    {
        internal readonly string value;
        KeyString _keyString = new KeyString(null,0);

        internal KeyString KeyString => _keyString.Value != null
            ? _keyString
            : (_keyString = KeyStrings.GetOrCreate(this.value));

        public JSString(string value): base(JSContext.Current.StringPrototype)
        {
            this.value = value;
        }

        public static implicit operator KeyString(JSString value)
        {
            return value.ToString();
        }

        

        public override string ToString()
        {
            return value;
        }

        public override string ToDetailString()
        {
            return value;
        }

        public override int Length => value.Length;

        public static JSValue Substring(JSValue t, JSArray a) 
        {
            var j = t as JSString;
            if (a.Length == 1)
                return new JSString(j.value.Substring(a[0].IntValue));
            if (a.Length == 2)
                return new JSString(j.value.Substring(a[0].IntValue, a[1].IntValue));
            return JSUndefined.Value;
        }

        internal static JSFunction Create()
        {
            var r = new JSFunction(JSFunction.empty);
            var p = r.prototype;

            p.DefineProperty(KeyStrings.length, JSProperty.Property(
                (t, a) => new JSNumber(t.Length),
                (t, a) => a[0]));
            
            p.DefineProperty(KeyStrings.toString, JSProperty.Function((t, a) => t));

            var substr = JSProperty.Function(Substring);
            p.DefineProperty(KeyStrings.GetOrCreate("substr"), substr);
            p.DefineProperty(KeyStrings.GetOrCreate("substring"), substr);

            p.DefineProperties(JSProperty.Function(KeyStrings.toString, (t, a) => t));
            return r;
        }

        public override JSValue AddValue(JSValue value)
        {
            return new JSString(this.value + value.ToString());
        }

        public override JSValue AddValue(double value)
        {
            return new JSString(this.value + value.ToString());
        }

        public override JSValue AddValue(string value)
        {
            return new JSString(this.value + value);
        }

        public override bool Equals(object obj)
        {
            if (obj is JSString v)
                return this.value == v.value;
            return base.Equals(obj);
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (value is JSString str)
                if (this.value == str.value)
                    return JSContext.Current.True;
            if (this.value == value.ToString())
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (value is JSString s)
                if (s.value == this.value)
                    return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue, JSArray args)
        {
            throw new NotImplementedException("string is not a function");
        }
    }
}
