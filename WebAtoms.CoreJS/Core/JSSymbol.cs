using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebAtoms.CoreJS.Core
{
    public class JSSymbol: JSValue
    {

        internal readonly KeyString Key;

        public JSSymbol(string name) : base(JSContext.Current.ObjectPrototype)
        {
            Key = KeyStrings.NewSymbol(name);
        }

        internal JSSymbol(KeyString k) : base(JSContext.Current.ObjectPrototype)
        {
            Key = k;
        }

        public override JSValue AddValue(JSValue value)
        {
            return new JSString(Key.Value + value.ToString());
        }

        public override JSValue AddValue(double value)
        {
            return new JSString(Key.Value + value.ToString());
        }

        public override JSValue AddValue(string value)
        {
            return new JSString(Key.Value + value);
        }

        public override bool Equals(object obj)
        {
            if (obj is JSSymbol s)
                return s.Key == Key;
            return false;
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (value == this)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override int GetHashCode()
        {
            return (int)Key.Key;
        }

        public override JSValue InvokeFunction(JSValue thisValue, JSArguments args)
        {
            throw new NotImplementedException("symbol is not a function");
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (value == this)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override string ToString()
        {
            return Key.Value;
        }

        internal override JSBoolean Greater(JSValue value)
        {
            return JSContext.Current.False;
        }

        internal override JSBoolean GreaterOrEqual(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        internal override JSBoolean Less(JSValue value)
        {
            return JSContext.Current.False;
        }

        internal override JSBoolean LessOrEqual(JSValue value)
        {
            if (object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            return JSContext.Current.False;
        }
    }
}
