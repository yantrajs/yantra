using System;
using System.Collections.Generic;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Typed;

namespace YantraJS.Core.Core.DataView
{
    [JSClassGenerator]
    public partial class DataView : JSObject
    {
        internal readonly JSArrayBuffer buffer;
        internal readonly int byteOffset;
        internal readonly int byteLength;

        public DataView(in Arguments a): this()
        {
            var buffer = a[0] as JSArrayBuffer ?? throw JSContext.CurrentContext.NewTypeError("First argument to DataView constructor must be an ArrayBuffer.");

            var byteOffset = a[1]?.IntValue ?? 0; //optional, if not available assign 0

            if (byteOffset >= buffer.buffer.Length)
                throw JSContext.CurrentContext.NewRangeError("Start offset is outside the bounds of the buffer.");

            var byteOffsetLength = buffer.buffer.Length - byteOffset;
            //optional, if not given it should be (buffer length - byte offset)
            var byteLength = a[2]?.IntValue ?? byteOffsetLength;

            if ((byteOffset + byteOffsetLength) > buffer.buffer.Length)
                throw JSContext.CurrentContext.NewTypeError("Invalid data view length.");

            this.buffer = buffer;
            this.byteLength = byteLength;
            this.byteOffset = byteOffset;
        }

        public DataView(
            JSArrayBuffer buffer,
            int byteOffset,
            int byteLength) : this()

        {
            this.buffer = buffer;
            this.byteLength = byteLength;
            this.byteOffset = byteOffset;
        }



        /// <summary>
        /// Stores a series of bytes at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="bytes"> The bytes to store. </param>
        /// <param name="littleEndian"> Indicates whether the bytes are stored in little- or
        /// big-endian format. If false, a big-endian value is written. </param>
        internal void SetCore(int byteOffset, byte[] bytes, bool littleEndian)
        {


            if (littleEndian)
            {
                for (int i = 0; i < bytes.Length; i++)
                    buffer.buffer[this.byteOffset + byteOffset + i] = bytes[i];
            }
            else
            {
                for (int i = 0; i < bytes.Length; i++)
                    buffer.buffer[this.byteOffset + byteOffset + bytes.Length - 1 - i] = bytes[i];
            }
        }
    }
}
