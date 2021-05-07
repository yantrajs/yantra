using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Typed;

namespace YantraJS.Core.Core.DataView
{
    [JSRuntime(typeof(DataViewStatic), typeof(DataViewPrototype))]
    public class DataView : JSObject
    {
        internal readonly JSArrayBuffer buffer;
        internal readonly int byteOffset;
        internal readonly int byteLength;



        public DataView(
            JSArrayBuffer buffer,
            int byteOffset,
            int byteLength) : base(JSContext.Current.DataViewPrototype)

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
