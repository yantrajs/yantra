using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using YantraJS.Core.Runtime;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    [JSRuntime(typeof(JSSymbolStatic), typeof(JSSymbolPrototype))]
    public class JSSymbol: JSValue
    {

        private static int SymbolID = 1;

        internal readonly KeyString Key;

        public override bool BooleanValue => true;

        public override double DoubleValue => throw JSContext.Current.NewTypeError("Cannot convert a Symbol value to a number.");

        internal override string StringValue => throw JSContext.Current.NewTypeError("Cannot convert a Symbol value to a string.");

        internal override PropertyKey ToKey(bool create = true)
        {
            return this;
        }

        public JSSymbol(string name) : base(JSContext.Current.ObjectPrototype)
        {
            Key = KeyStrings.NewSymbol(name, (uint)Interlocked.Increment(ref SymbolID));
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
            return Key.Value.Value;
        }


    }
}
