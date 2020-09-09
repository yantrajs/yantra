using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebAtoms.CoreJS.Core
{
    public class JSSymbol: JSValue
    {

        internal readonly KeyString Key;

        public override bool BooleanValue => true;

        internal override KeyString ToKey()
        {
            return Key;
        }

        public JSSymbol(string name) : base(JSContext.Current.ObjectPrototype)
        {
            Key = KeyStrings.NewSymbol(name);
        }

        internal JSSymbol(KeyString k) : base(JSContext.Current.ObjectPrototype)
        {
            Key = k;
        }

        public override JSValue TypeOf()
        {
            return JSConstants.Symbol;
        }

        public override bool Equals(object obj)
        {
            if (obj is JSSymbol s)
                return s.Key.Key == Key.Key;
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

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
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

    }
}
