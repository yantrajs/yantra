using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
                throw JSContext.CurrentContext.NewRangeError("Start offset is outside the bounds of the buffer.");

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


        /// <summary>
        /// Gets a signed 64-bit integer at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The signed 64-bit integer at the specified byte offset from the start
        /// of the DataView. </returns>
        [Prototype("getBigInt64", Length =1)]
        public static JSValue GetBigInt64(in Arguments a)
        {
            return new JSNumber(GetInt64(in a));
        }

        //internal method
        public unsafe static long GetInt64(in Arguments a)
        {
            var @this = a.This.AsDataView();
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewTypeError($"offset is required");
            var littleEndian = a[1]?.BooleanValue ?? false;
            if (byteOffset < 0 || byteOffset > @this.byteLength - 8)
                throw JSContext.Current.NewRangeError($"Offset {byteOffset} is outside the bounds of DataView");
            var buffer = @this.buffer;
            fixed (byte* ptr = &buffer.buffer[@this.byteOffset + byteOffset])
            {
                if (littleEndian)
                {
                    int temp1 = (*ptr) | (*(ptr + 1) << 8) | (*(ptr + 2) << 16) | (*(ptr + 3) << 24);
                    int temp2 = (*(ptr + 4)) | (*(ptr + 5) << 8) | (*(ptr + 6) << 16) | (*(ptr + 7) << 24);
                    return (uint)temp1 | ((long)temp2 << 32);
                }
                else
                {
                    int temp1 = (*ptr << 24) | (*(ptr + 1) << 16) | (*(ptr + 2) << 8) | (*(ptr + 3));
                    int temp2 = (*(ptr + 4) << 24) | (*(ptr + 5) << 16) | (*(ptr + 6) << 8) | (*(ptr + 7));
                    return (uint)temp2 | ((long)temp1 << 32);
                }
            }
        }

        [Prototype("getBigUInt64", Length = 1)]
        public static JSValue GetBigUInt64(in Arguments a) {
            return new JSNumber(GetInt64(in a));
        }

        //internal method
        public unsafe static int GetInt32Int(in Arguments a)
        {
            var @this = a.This.AsDataView();
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewTypeError($"offset is required");
            var littleEndian = a[1]?.BooleanValue ?? false;
            if (byteOffset < 0 || byteOffset > @this.byteLength - 4)
                throw JSContext.Current.NewRangeError($"Offset {byteOffset} is outside the bounds of DataView");
            var buffer = @this.buffer;

            fixed (byte* ptr = &buffer.buffer[@this.byteOffset + byteOffset])
            {
                if (littleEndian)
                {
                    return (*ptr) | (*(ptr + 1) << 8) | (*(ptr + 2) << 16) | (*(ptr + 3) << 24);
                }
                else
                {
                    return (*ptr << 24) | (*(ptr + 1) << 16) | (*(ptr + 2) << 8) | (*(ptr + 3));
                }
            }
        }

        /// <summary>
        /// Gets a 32-bit floating point number at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The 32-bit floating point number at the specified byte offset from the start
        /// of the DataView. </returns>
        [Prototype("getFloat32", Length = 2)]
        public unsafe static JSValue GetFloat32(in Arguments a)
        { 
            int temp = GetInt32Int(in a);
            return new JSNumber(*(float*)&temp);
        }

        /// <summary>
        /// Gets a 64-bit floating point number at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The 64-bit floating point number at the specified byte offset from the start
        /// of the DataView. </returns>
        [Prototype("getFloat64", Length = 2)]
        public unsafe static JSValue GetFloat64(in Arguments a) {
            long temp = GetInt64(in a);
            return new JSNumber(*(double*) & temp);
        }

       
        //internal
        public unsafe static int GetInt16Int(in Arguments a)
        {
            var @this = a.This.AsDataView();
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewTypeError($"offset is required");
            var littleEndian = a[1]?.BooleanValue ?? false;
            if (byteOffset < 0 || byteOffset > @this.byteLength - 2)
                throw JSContext.Current.NewRangeError($"Offset {byteOffset} is outside the bounds of DataView");
            var buffer = @this.buffer;
            fixed (byte* ptr = &buffer.buffer[@this.byteOffset + byteOffset])
            {
                if (littleEndian)
                {
                    return (*ptr) | (*(ptr + 1) << 8) | (*(ptr + 2) << 16) | (*(ptr + 3) << 24);
                }
                else
                {
                    return (*ptr << 24) | (*(ptr + 1) << 16) | (*(ptr + 2) << 8) | (*(ptr + 3));
                }
            }
        }


        /// <summary>
        /// Gets a signed 16-bit integer at the specified byte offset from the start of the DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The signed 16-bit integer at the specified byte offset from the start of the
        /// DataView. </returns>
        [Prototype("getInt16", Length = 2)]
        public static JSValue GetInt16(in Arguments a)
        {
            return new JSNumber(GetInt16Int(in a));
        }


        /// <summary>
        /// Gets a signed 32-bit integer at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The signed 32-bit integer at the specified byte offset from the start
        /// of the DataView. </returns>
        [Prototype("getInt32", Length = 2)]
        public static JSValue GetInt32(in Arguments a) {
            return new JSNumber(GetInt32Int(in a));
        }


        /// <summary>
        /// Gets a signed 8-bit integer (byte) at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <returns> The signed 8-bit integer (byte) at the specified byte offset from the start
        /// of the DataView. </returns>
        [Prototype("getInt8", Length = 1)]
        public static JSValue GetInt8(in Arguments a)
        {
            return new JSNumber(GetInt8Int(in a));
        }

        public static int GetInt8Int(in Arguments a)
        {
            var @this = a.This.AsDataView();
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewTypeError($"Offset is required");
            if (byteOffset < 0 || byteOffset > @this.byteLength - 1)
                throw JSContext.Current.NewRangeError($"Offset {byteOffset} is outside the bounds of DataView");
            var buffer = @this.buffer;
            return (sbyte)buffer.buffer[@this.byteOffset + byteOffset];
        }


        /// <summary>
        /// Gets an unsigned 8-bit integer (byte) at the specified byte offset from the start of
        /// the DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The unsigned 8-bit integer (byte) at the specified byte offset from the start
        /// of the DataView. </returns>
        [Prototype("getUint16", Length = 2)]
        public static JSValue GetUint16(in Arguments a) {
            return new JSNumber((ushort)GetInt16Int(in a));
        }


        /// <summary>
        /// Gets an unsigned 32-bit integer at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <param name="littleEndian"> Indicates whether the number is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is read. </param>
        /// <returns> The unsigned 32-bit integer at the specified byte offset from the start
        /// of the DataView. </returns>
        [Prototype("getUint32", Length = 2)]
        public static JSValue GetUint32(in Arguments a)
        {
            return new JSNumber((uint)GetInt32Int(in a));
        }


        /// <summary>
        /// Gets an unsigned 8-bit integer (byte) at the specified byte offset from the start of
        /// the DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// read the data. </param>
        /// <returns> The unsigned 8-bit integer (byte) at the specified byte offset from the start
        /// of the DataView. </returns>
        [Prototype("getUint8", Length = 1)]
        public static JSValue GetUint8(in Arguments a)
        {
            var @this = a.This.AsDataView();
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewTypeError($"offset is required");
            if (byteOffset < 0 || byteOffset > @this.byteLength - 1)
                throw JSContext.Current.NewTypeError($"{byteOffset} offset is outside the bounds of DataView");
            var buffer = @this.buffer;
            return new JSNumber( buffer.buffer[@this.byteOffset + byteOffset]);
        }

        /// <summary>
        /// Stores a signed 64-bit float value at the specified byte offset from the start of the
        /// DataView.
        /// </summary>
        /// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
        /// store the data. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="littleEndian"> Indicates whether the 64-bit float is stored in little- or
        /// big-endian format. If false or undefined, a big-endian value is written. </param>
        [Prototype("setBigInt64", Length = 2)]
        public static JSValue SetBigInt64(in Arguments a) {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 8);
            var bytes = BitConverter.GetBytes(value.BigIntValue);
            @this.SetCore(byteOffset,bytes ,littleEndian);
            return JSUndefined.Value;
        }

        [Prototype("setBigUInt64", Length = 2)]
        public static JSValue SetBigUInt64(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 8);
            var bytes = BitConverter.GetBytes((ulong)value.DoubleValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [Prototype("setFloat32", Length = 3)]
        public static JSValue SetFloat32(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 4);
            var bytes = BitConverter.GetBytes((float)value.DoubleValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }


        [Prototype("setFloat64", Length = 3)]
        public static JSValue SetFloat64(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 8);
            var bytes = BitConverter.GetBytes(value.DoubleValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [Prototype("setInt16", Length = 2)]
        public static JSValue SetInt16(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 2);
            var bytes = BitConverter.GetBytes((short)value.IntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [Prototype("setInt32", Length = 3)]
        public static JSValue SetInt32(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 4);
            var bytes = BitConverter.GetBytes(value.IntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }


        [Prototype("setInt8", Length = 2)]
        public static JSValue SetInt8(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 1);
            var bytes = BitConverter.GetBytes((sbyte)value.IntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [Prototype("setUint16", Length = 2)]
        public static JSValue SetUint16(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 2);
            var bytes = BitConverter.GetBytes((ushort)value.IntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [Prototype("setUint32", Length = 3)]
        public static JSValue SetUint32(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 4);
            var bytes = BitConverter.GetBytes((uint)value.IntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [Prototype("setUint8", Length = 2)]
        public static JSValue SetUint8(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 1);
            var bytes = BitConverter.GetBytes((byte)value.IntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (int byteOffset, bool littleEndian, DataView dataView, JSValue value) GetSetArgs(in Arguments a, int length)
        {
            var @this = a.This.AsDataView();
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewRangeError($"offset is required");
            var value = a[1] ?? throw JSContext.Current.NewTypeError($"value is required");
            
            var littleEndian = a[2]?.BooleanValue ?? false;
            if (byteOffset < 0 || byteOffset > @this.byteLength - length)
                throw JSContext.Current.NewRangeError($"Offset {byteOffset} is outside the bounds of DataView");

            return (byteOffset, littleEndian, @this, value);
            // return JSUndefined.Value;
        }


    }
}
