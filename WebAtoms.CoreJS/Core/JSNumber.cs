using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public sealed class JSNumber : JSValue
    {

        private double value;

        internal JSNumber(double value)
        {
            this.value = value;
        }

        public override int IntValue => (int)value;

        public override double DoubleValue => value;

        public override string ToString()
        {
            return value.ToString();
        }

    }
}
