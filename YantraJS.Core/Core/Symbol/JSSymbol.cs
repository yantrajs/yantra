using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Yantra.Core;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    // [JSRuntime(typeof(JSSymbolStatic), typeof(JSSymbolPrototype))]
    [JSBaseClass("Object")]
    [JSFunctionGenerator("Symbol")]
    public partial class JSSymbol: JSValue
    {

        private static int SymbolID = 1;
        private readonly string name;
        public readonly uint Key;

        public override bool BooleanValue => true;

        public override bool IsSymbol => true;

        public override double DoubleValue => throw JSContext.Current.NewTypeError("Cannot convert a Symbol value to a number.");

        internal override string StringValue => throw JSContext.Current.NewTypeError("Cannot convert a Symbol value to a string.");

        public override uint UIntValue => throw JSContext.Current.NewTypeError("Cannot convert a Symbol value to a uint32.");

        internal override PropertyKey ToKey(bool create = true)
        {
            return this;
        }

        public JSSymbol(string name) : base(JSContext.Current.ObjectPrototype)
        {
            this.name = name;
            Key = (uint)Interlocked.Increment(ref SymbolID);
        }

        public override JSValue TypeOf()
        {
            return JSConstants.Symbol;
        }

        public override bool Equals(object obj)
        {
            if (obj is JSSymbol s)
                return s.Key == Key;
            return false;
        }

        public override bool Equals(JSValue value)
        {
            return ReferenceEquals(this, value);
            //if (value == this)
            //    return JSBoolean.True;
            //return JSBoolean.False;
        }

        public override int GetHashCode()
        {
            return (int)Key;
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

        public override bool StrictEquals(JSValue value)
        {
            return ReferenceEquals(this, value);
        }

        public override string ToString()
        {
            return name;
        }


    }
}
