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

        public override JSValue this[JSValue key]
        {
            get => throw new InvalidOperationException($"Cannot get {key} of {this}");
            set => throw new InvalidOperationException($"Cannot set {key} of {this}");
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static JSObject CreatePrototype(JSContext context, JSObject prototype)
        {
            var number = new JSObject();
            number.prototype = prototype;
            context[KeyStrings.Number] = number;
            return number;

        }
    }
}
