using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Core.Array
{
    public class JSSpreadValue : JSValue
    {
        private int _length;

        public JSSpreadValue(JSValue value) : base(JSValueType.Spread)
        {
            this.Value = value;
            _length = value.Length;
        }

        public override int Length { get => _length; set { } }

        //public override JSValue this[uint key] { 
        //    get => Value[key]; 
        //    set => Value[key] = value; 
        //}

        protected internal override JSValue GetValue(uint key, JSValue receiver, bool throwError = true)
        {
            return Value[key];
        }

        protected internal override bool SetValue(uint key, JSValue value, JSValue receiver, bool throwError = true)
        {
            Value[key] = value;
            return true;
        }


        public override bool BooleanValue => throw new NotImplementedException();

        public JSValue Value { get; }

        public override bool Equals(JSValue value)
        {
            throw new NotImplementedException();
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException();
        }

        public override bool StrictEquals(JSValue value)
        {
            throw new NotImplementedException();
        }

        // public override JSValue TypeOf()
        // {
        //     throw new NotImplementedException();
        // }

        internal override PropertyKey ToKey(bool create = true)
        {
            throw new NotImplementedException();
        }
    }
}
