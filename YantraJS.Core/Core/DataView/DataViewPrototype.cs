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
            //optional, if not given it should be (buffer length - byte offset)
            var byteLength = a[2]?.IntValue ?? byteOffsetLength;  

            if ((byteOffset + byteOffsetLength) > buffer.buffer.Length)
                throw JSContext.CurrentContext.NewTypeError("Invalid data view length.");

            return new DataView(buffer, byteOffset, byteLength);


        }

        //property
        [GetProperty("buffer")]
        public static JSValue Buffer(in Arguments a)
        {
            var @this = a.This.AsDataView();
            return @this.buffer;
        }

        //property
        [GetProperty("byteLength")]
        public static JSValue ByteLength(in Arguments a)
        {
            var @this = a.This.AsDataView();
            return new JSNumber(@this.byteLength);
        }

        //property
        [GetProperty("byteOffset")]
        public static JSValue ByteOffset(in Arguments a)
        {
            var @this = a.This.AsDataView();
            return new JSNumber(@this.byteOffset);
        }


        [GetProperty("getBigInt64", Length =1)]
        public unsafe static JSValue GetBigInt64(in Arguments a)
        {
            var @this = a.This.AsDataView();
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewTypeError($"offset is required");
            var littleEndian = a[1]?.BooleanValue ?? false;
            if(byteOffset < 0 || byteOffset > @this.byteLength - 8  )
                throw JSContext.Current.NewTypeError($"{byteOffset} is outside the bounds of DataView");
            var buffer = @this.buffer;
            fixed (byte* ptr = &buffer.buffer[@this.byteOffset + byteOffset])
            {
                if (littleEndian)
                {
                    int temp1 = (*ptr) | (*(ptr + 1) << 8) | (*(ptr + 2) << 16) | (*(ptr + 3) << 24);
                    int temp2 = (*(ptr + 4)) | (*(ptr + 5) << 8) | (*(ptr + 6) << 16) | (*(ptr + 7) << 24);
                    return new JSNumber((uint)temp1 | ((long)temp2 << 32));
                }
                else
                {
                    int temp1 = (*ptr << 24) | (*(ptr + 1) << 16) | (*(ptr + 2) << 8) | (*(ptr + 3));
                    int temp2 = (*(ptr + 4) << 24) | (*(ptr + 5) << 16) | (*(ptr + 6) << 8) | (*(ptr + 7));
                    return new JSNumber((uint)temp2 | ((long)temp1 << 32));
                }
            }
        }


    }
}
