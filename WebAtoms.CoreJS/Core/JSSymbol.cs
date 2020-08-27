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

        public override JSValue Add(JSValue value)
        {
            return new JSString(Key.Value + value.ToString());
        }

        public override JSValue Add(double value)
        {
            return new JSString(Key.Value + value.ToString());
        }

        public override JSValue Add(string value)
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
    }
}
