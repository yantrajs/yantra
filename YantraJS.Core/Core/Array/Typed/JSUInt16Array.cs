using System;
using Yantra.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Core.Typed
{
    [JSClassGenerator("Uint16Array"), JSBaseClass("TypedArray")]
    public partial class JSUInt16Array : JSTypedArray
    {

        [JSExport("BYTES_PER_ELEMENT")]
        internal static readonly int BYTES_PER_ELENENT = 2;


        [JSExport(Length = 3)]
        public JSUInt16Array(in Arguments a)
            : base(new TypedArrayParameters(a, BYTES_PER_ELENENT))
        {
        }

        private JSUInt16Array(TypedArrayParameters a) : base(a)
        {

        }

        protected internal override JSValue GetValue(uint index, JSValue receiver, bool throwError = true)
        {
            if (index < 0 || index >= this.length)
                return JSUndefined.Value;
            return new JSNumber((int)BitConverter.ToUInt16(this.buffer.buffer, this.byteOffset + (int)index * 2));
        }

        internal protected override bool SetValue(uint index, JSValue value, JSValue receiver, bool throwError = true)
        {
            if (index < 0 || index >= this.length)
                return false;
            Array.Copy(BitConverter.GetBytes((UInt16)value.IntValue), 0, this.buffer.buffer, this.byteOffset + index * 2, 2);
            return true;
        }

        [JSExport(Length = 1)]
        public static JSValue From(in Arguments a)
        {
            return new JSUInt16Array(TypedArrayParameters.From(in a, BYTES_PER_ELENENT));
        }

        [JSExport]
        public static JSValue Of(in Arguments a)
        {
            var r = new JSUInt16Array(TypedArrayParameters.Of(in a, BYTES_PER_ELENENT));
            for (int i = 0; i < a.Length; i++)
            {
                r[(uint)i] = a[i];
            }
            return r;
        }
    }
}
