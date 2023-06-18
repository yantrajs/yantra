using System;
using Yantra.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Core.Typed
{
    [JSClassGenerator("Uint8ClampedArray"), JSBaseClass("TypedArray")]
    public partial class JSUint8ClampedArray : JSTypedArray
    {

        [JSExport("BYTES_PER_ELEMENT")]
        internal static readonly int BYTES_PER_ELENENT = 1;


        [JSExport(Length = 3)]
        public JSUint8ClampedArray(in Arguments a)
            : base(new TypedArrayParameters(a, BYTES_PER_ELENENT))
        {
        }

        private JSUint8ClampedArray(TypedArrayParameters a) : base(a)
        {

        }

        protected internal override JSValue GetValue(uint index, JSValue receiver, bool throwError = true)
        {
            if (index < 0 || index >= this.length)
                return JSUndefined.Value;
            return new JSNumber((int)this.buffer.buffer[this.byteOffset + index]);
        }

        internal protected override bool SetValue(uint index, JSValue value, JSValue receiver, bool throwError = true)
        {
            if (index < 0 || index >= this.length)
                return false;
            // This algorithm is defined as ToUint8Clamp in the spec.
            double number = value.DoubleValue;
            int result;
            if (number <= 0)
                result = 0;
            else if (number >= 255)
                result = 255;
            else
            {
                var f = Math.Floor(number);
                if (f + 0.5 < number)
                    result = (int)f + 1;
                else if (number < f + 0.5)
                    result = (int)f;
                else if ((int)f % 2 == 0)
                    result = (int)f;
                else
                    result = (int)f + 1;
            }
            this.buffer.buffer[this.byteOffset + index] = (byte)result;
            return true;
        }

        [JSExport(Length = 1)]
        public static JSValue From(in Arguments a)
        {
            return new JSUint8ClampedArray(TypedArrayParameters.From(in a, BYTES_PER_ELENENT));
        }

        [JSExport]
        public static JSValue Of(in Arguments a)
        {
            var r = new JSUint8ClampedArray(TypedArrayParameters.Of(in a, BYTES_PER_ELENENT));
            for (int i = 0; i < a.Length; i++)
            {
                r[(uint)i] = a[i];
            }
            return r;
        }
    }
}
