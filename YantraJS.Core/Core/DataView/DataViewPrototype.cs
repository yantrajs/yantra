using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Typed;

namespace YantraJS.Core.Core.DataView
{
    class DataViewPrototype 
    {

        [Constructor(Length = 3)]
        public static JSValue Constructor(in Arguments a)
        {
            var buffer = a[0] as JSArrayBuffer ?? throw JSContext.CurrentContext.NewTypeError("First argument to DataView constructor must be an ArrayBuffer.");

            var byteOffset = a[1]?.IntValue ?? 0; //optional, if not available assign 0

            if (byteOffset >= buffer.buffer.Length)
                throw JSContext.CurrentContext.NewTypeError("Start offset is outside the bounds of the buffer.");

            var byteOffsetLength = buffer.buffer.Length - byteOffset;
            var byteLength = a[2]?.IntValue ?? byteOffsetLength;  //optional, if not given it should be null

            if ((byteOffset + byteOffsetLength) > buffer.buffer.Length)
                throw JSContext.CurrentContext.NewTypeError("Invalid data view length.");

            return new DataView(buffer, byteOffset, byteLength);


        }


        [GetProperty("buffer")]
        public static JSValue Buffer(in Arguments a)
        {
            var @this = a.This;
            return @this.buffer;
        }
    }
}
