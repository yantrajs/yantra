using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    [JSRuntime(typeof(JSSymbolStatic), typeof(JSSymbolPrototype))]
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
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override int GetHashCode()
        {
            return (int)Key.Key;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            var f = a.Get1();
            if (f.IsUndefined)
                return new JSSymbol("");
            return new JSSymbol(a.ToString());
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            throw new NotSupportedException();
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (value == this)
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override string ToString()
        {
            return Key.Value;
        }

        internal override IEnumerable<JSValue> AllElements => throw new NotImplementedException();

    }
}
