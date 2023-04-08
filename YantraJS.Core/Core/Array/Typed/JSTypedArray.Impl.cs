using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Core.Typed
{

    [JSClassGenerator("Float32Array"), JSBaseClass("TypedArray")]
    public partial class JSFloat32Array : JSTypedArray
    {

        [JSExport("BYTES_PER_ELEMENT")]
        internal static readonly int BYTES_PER_ELENENT = 4;


        [JSExport(Length = 3)]
        public JSFloat32Array(in Arguments a)
            : base(new TypedArrayParameters(a, 1))
        {
        }

        private JSFloat32Array(TypedArrayParameters a) : base(a)
        {

        }

        protected internal override JSValue GetValue(uint index, JSValue receiver, bool throwError = true)
        {
            if (index < 0 || index >= this.length)
                return JSUndefined.Value;
            return new JSNumber((double)BitConverter.ToSingle(this.buffer.buffer, this.byteOffset + (int)index * 4));
        }

        internal protected override bool SetValue(uint index, JSValue value, JSValue receiver, bool throwError = true)
        {
            if (index < 0 || index >= this.length)
                return false;
            Array.Copy(BitConverter.GetBytes((float)value.DoubleValue), 0, this.buffer.buffer, this.byteOffset + index * 4, 4);
            return true;
        }

        [JSExport]
        public static JSValue From(in Arguments a)
        {
            var (f, map, mapThis) = a.Get3();
            return new JSFloat32Array(new TypedArrayParameters(f, map, mapThis, BYTES_PER_ELENENT));
        }

        [JSExport]
        public static JSValue Of(in Arguments a)
        {
            var r = new JSFloat32Array(new TypedArrayParameters(a.Length, BYTES_PER_ELENENT));
            for (int i = 0; i < a.Length; i++)
            {
                r[(uint)i] = a[i];
            }
            return r;
        }
    }

    [JSClassGenerator("Float64Array"), JSBaseClass("TypedArray")]
    public partial class JSFloat64Array : JSTypedArray
    {

        [JSExport("BYTES_PER_ELEMENT")]
        internal static readonly int BYTES_PER_ELENENT = 4;


        [JSExport(Length = 3)]
        public JSFloat64Array(in Arguments a)
            : base(new TypedArrayParameters(a, 1))
        {
        }

        private JSFloat64Array(TypedArrayParameters a) : base(a)
        {

        }

        protected internal override JSValue GetValue(uint index, JSValue receiver, bool throwError = true)
        {
            if (index < 0 || index >= this.length)
                return JSUndefined.Value;
            return new JSNumber(BitConverter.ToDouble(this.buffer.buffer, this.byteOffset + (int)index * 8));
        }

        internal protected override bool SetValue(uint index, JSValue value, JSValue receiver, bool throwError = true)
        {
            if (index < 0 || index >= this.length)
                return false;
            Array.Copy(BitConverter.GetBytes(value.DoubleValue), 0, this.buffer.buffer, this.byteOffset + index * 8, 8);
            return true;
        }

        [JSExport]
        public static JSValue From(in Arguments a)
        {
            var (f, map, mapThis) = a.Get3();
            return new JSFloat64Array(new TypedArrayParameters(f, map, mapThis, BYTES_PER_ELENENT));
        }

        [JSExport]
        public static JSValue Of(in Arguments a)
        {
            var r = new JSFloat64Array(new TypedArrayParameters(a.Length, BYTES_PER_ELENENT));
            for (int i = 0; i < a.Length; i++)
            {
                r[(uint)i] = a[i];
            }
            return r;
        }
    }
}
