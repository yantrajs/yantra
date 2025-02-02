using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Typed;

namespace YantraJS.Core.Core.DataView
{
    [JSClassGenerator]
    public partial class DataView : JSObject
    {
        internal readonly JSArrayBuffer buffer;
        internal readonly int byteOffset;
        internal readonly int byteLength;

        [JSExport(Length = 3)]
        public DataView(in Arguments a): this(JSContext.NewTargetPrototype)
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

        [JSExport]
        public JSValue Buffer => this.buffer;

        [JSExport]
        public int ByteLength => this.byteLength;

        [JSExport]
        public int ByteOffset => this.byteOffset;


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
        [JSExport(Length = 1)]
        public JSValue GetBigInt64(in Arguments a)
        {
            return new JSNumber(GetInt64(in a));
        }

        //internal method
        public unsafe long GetInt64(in Arguments a)
        {
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewTypeError($"offset is required");
            var littleEndian = a[1]?.BooleanValue ?? false;
            if (byteOffset < 0 || byteOffset > byteLength - 8)
                throw JSContext.Current.NewRangeError($"Offset {byteOffset} is outside the bounds of DataView");
            fixed (byte* ptr = &buffer.buffer[byteOffset + byteOffset])
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

        [JSExport(Length = 1)]
        public JSValue GetBigUInt64(in Arguments a)
        {
            return new JSNumber(GetInt64(in a));
        }

        //internal method
        public unsafe int GetInt32Int(in Arguments a)
        {
            var @this = this;
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
        [JSExport(Length = 2)]
        public unsafe JSValue GetFloat32(in Arguments a)
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
        [JSExport(Length = 2)]
        public unsafe JSValue GetFloat64(in Arguments a)
        {
            long temp = GetInt64(in a);
            return new JSNumber(*(double*)&temp);
        }


        //internal
        public unsafe int GetInt16Int(in Arguments a)
        {
            var @this = this;
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewTypeError($"offset is required");
            var littleEndian = a[1]?.BooleanValue ?? false;
            if (byteOffset < 0 || byteOffset > @this.byteLength - 2)
                throw JSContext.Current.NewRangeError($"Offset {byteOffset} is outside the bounds of DataView");
            var buffer = @this.buffer;
            fixed (byte* ptr = &buffer.buffer[@this.byteOffset + byteOffset])
            {
                if (littleEndian)
                {
                    return (short)((*ptr) | (*(ptr + 1) << 8));
                }
                else
                {
                    return (short)((*ptr << 8) | (*(ptr + 1)));
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
        [JSExport(Length = 2)]
        public JSValue GetInt16(in Arguments a)
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
        [JSExport(Length = 2)]
        public JSValue GetInt32(in Arguments a)
        {
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
        [JSExport(Length = 1)]
        public JSValue GetInt8(in Arguments a)
        {
            return new JSNumber(GetInt8Int(in a));
        }

        public int GetInt8Int(in Arguments a)
        {
            var @this = this;
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
        [JSExport(Length = 2)]
        public JSValue GetUint16(in Arguments a)
        {
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
        [JSExport(Length = 2)]
        public JSValue GetUint32(in Arguments a)
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
        [JSExport(Length = 1)]
        public JSValue GetUint8(in Arguments a)
        {
            var @this = this;
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewTypeError($"offset is required");
            if (byteOffset < 0 || byteOffset > @this.byteLength - 1)
                throw JSContext.Current.NewRangeError($"{byteOffset} offset is outside the bounds of DataView");
            var buffer = @this.buffer;
            return new JSNumber(buffer.buffer[@this.byteOffset + byteOffset]);
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
        [JSExport(Length = 2)]
        public JSValue SetBigInt64(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 8);
            var bytes = BitConverter.GetBytes(value.BigIntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [JSExport(Length = 2)]
        public JSValue SetBigUInt64(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 8);
            var bytes = BitConverter.GetBytes((ulong)value.DoubleValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [JSExport(Length = 3)]
        public JSValue SetFloat32(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 4);
            var bytes = BitConverter.GetBytes((float)value.DoubleValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }


        [JSExport(Length = 3)]
        public JSValue SetFloat64(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 8);
            var bytes = BitConverter.GetBytes(value.DoubleValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [JSExport(Length = 3)]
        public JSValue SetInt16(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 2);
            var bytes = BitConverter.GetBytes((short)(uint)value.IntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [JSExport(Length = 3)]
        public JSValue SetInt32(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 4);
            var bytes = BitConverter.GetBytes(value.IntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }


        [JSExport(Length = 2)]
        public JSValue SetInt8(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 1);
            var bytes = (byte)(sbyte)(uint)value.IntValue;
            @this.buffer.buffer[@this.byteOffset + byteOffset] = bytes;
            return JSUndefined.Value;
        }

        [JSExport(Length = 3)]
        public JSValue SetUint16(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 2);
            var bytes = BitConverter.GetBytes((ushort)value.IntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [JSExport(Length = 3)]
        public JSValue SetUint32(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 4);
            var bytes = BitConverter.GetBytes((uint)value.IntValue);
            @this.SetCore(byteOffset, bytes, littleEndian);
            return JSUndefined.Value;
        }

        [JSExport(Length = 2)]
        public JSValue SetUint8(in Arguments a)
        {
            var (byteOffset, littleEndian, @this, value) = GetSetArgs(in a, 1);
            var bytes = (byte)(uint)value.IntValue;
            @this.buffer.buffer[@this.byteOffset + byteOffset] = bytes;

            return JSUndefined.Value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (int byteOffset, bool littleEndian, DataView dataView, JSValue value) GetSetArgs(in Arguments a, int length)
        {
            var @this = this;
            var byteOffset = a[0]?.IntValue ?? throw JSContext.Current.NewTypeError($"offset is required");
            var value = a[1] ?? throw JSContext.Current.NewTypeError($"value is required");

            var littleEndian = a[2]?.BooleanValue ?? false;
            if (byteOffset < 0 || byteOffset > @this.byteLength - length)
                throw JSContext.Current.NewRangeError($"Offset {byteOffset} is outside the bounds of DataView");
            return (byteOffset, littleEndian, @this, value);
        }



    }
}
