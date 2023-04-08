using Yantra.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Core.Typed
{
    [JSClassGenerator("UInt8Array"), JSBaseClass("TypedArray")]
    public partial class JSUInt8Array : JSTypedArray
    {

        [JSExport("BYTES_PER_ELEMENT")]
        internal static readonly int BYTES_PER_ELENENT = 1;


        [JSExport(Length = 3)]
        public JSUInt8Array(in Arguments a)
            : base(new TypedArrayParameters(a, 1))
        {
        }

        private JSUInt8Array(TypedArrayParameters a) : base(a)
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
            this.buffer.buffer[this.byteOffset + index] = (byte)(uint)value.IntValue;
            return true;
        }

        [JSExport]
        public static JSValue From(in Arguments a)
        {
            var (f, map, mapThis) = a.Get3();
            return new JSUInt8Array(new TypedArrayParameters(f, map, mapThis, BYTES_PER_ELENENT));
        }

        [JSExport]
        public static JSValue Of(in Arguments a)
        {
            var r = new JSUInt8Array(new TypedArrayParameters(a.Length, BYTES_PER_ELENENT));
            for (int i = 0; i < a.Length; i++)
            {
                r[(uint)i] = a[i];
            }
            return r;
        }
    }
}
