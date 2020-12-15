using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Core.Array
{
    public class JSSpreadValue : JSValue
    {
        private int _length;

        public JSSpreadValue(JSValue value) : base(null)
        {
            this.Value = value;
            _length = value.Length;
        }

        internal override bool IsSpread => true;


        public override bool BooleanValue => throw new NotImplementedException();

        public JSValue Value { get; }

        public override JSBoolean Equals(JSValue value)
        {
            throw new NotImplementedException();
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException();
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            throw new NotImplementedException();
        }

        public override JSValue TypeOf()
        {
            throw new NotImplementedException();
        }

        internal override KeyString ToKey(bool create = true)
        {
            throw new NotImplementedException();
        }
    }
}
