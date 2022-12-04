﻿using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Generator;

namespace YantraJS.Core.Typed
{
    /// <summary>
    /// Defines the element type and behaviour of typed array.
    /// </summary>
    public enum TypedArrayType
    {
        /// <summary>
        /// An array of signed 8-bit elements.
        /// </summary>
        Int8Array,

        /// <summary>
        /// An array of unsigned 8-bit elements.
        /// </summary>
        Uint8Array,

        /// <summary>
        /// An array of unsigned 8-bit elements, clamped to 0-255.
        /// </summary>
        Uint8ClampedArray,

        /// <summary>
        /// An array of signed 16-bit elements.
        /// </summary>
        Int16Array,

        /// <summary>
        /// An array of unsigned 16-bit elements.
        /// </summary>
        Uint16Array,

        /// <summary>
        /// An array of signed 32-bit elements.
        /// </summary>
        Int32Array,

        /// <summary>
        /// An array of unsigned 32-bit elements.
        /// </summary>
        Uint32Array,

        /// <summary>
        /// An array of 32-bit floating point elements.
        /// </summary>
        Float32Array,

        /// <summary>
        /// An array of 64-bit floating point elements.
        /// </summary>
        Float64Array,
    }


    public class TypedArray : JSObject
    {
        internal readonly JSArrayBuffer buffer;
        internal readonly TypedArrayType type;
        internal readonly int byteOffset;
        internal readonly int bytesPerElement;
        private readonly int length;

        public override int Length { get => length; set => base.Length = value; }

        public TypedArray(
            JSArrayBuffer buffer,
            TypedArrayType type,
            int byteOffset, 
            int length,   
            JSObject prototype) : base(prototype)
        {
            switch (type)
            {
                case TypedArrayType.Int8Array:
                    bytesPerElement = 1;
                    break;
                case TypedArrayType.Uint8Array:
                    bytesPerElement = 1;
                    break;
                case TypedArrayType.Uint8ClampedArray:
                    bytesPerElement = 1;
                    break;
                case TypedArrayType.Int16Array:
                    bytesPerElement = 2;
                    break;
                case TypedArrayType.Uint16Array:
                    bytesPerElement = 2;
                    break;
                case TypedArrayType.Int32Array:
                    bytesPerElement = 4;
                    break;
                case TypedArrayType.Uint32Array:
                    bytesPerElement = 4;
                    break;
                case TypedArrayType.Float32Array:
                    bytesPerElement = 4;
                    break;
                case TypedArrayType.Float64Array:
                    bytesPerElement = 8;
                    break;
            }

            if (buffer == null) {
                buffer = new JSArrayBuffer(length * bytesPerElement);
                this.length = length;
            }
            else { 
                if (length == -1)
                {
                    length = buffer.buffer.Length - byteOffset;
                    this.length = length / bytesPerElement;
                } else
                {
                    this.length = length / bytesPerElement;
                }

                if ( length < 0 ||
                    ((byteOffset + length) > buffer.buffer.Length))
                    throw JSContext.Current.NewRangeError($"Start offset {byteOffset} is outside the bounds of the buffer");

                if (((length - byteOffset) % bytesPerElement) != 0)
                {
                    throw JSContext.Current.NewRangeError($"byte length of {this.GetType().Name} should be multiple of {bytesPerElement}");
                }

            }

            this.buffer = buffer;
            this.type = type;
            this.byteOffset = byteOffset;
        }

        internal override JSValue GetValue(uint index, JSValue receiver, bool throwError = true)
        {
            if (index < 0 || index >= this.length)
                return JSUndefined.Value;
            switch (type)
            {
                case TypedArrayType.Int8Array:
                    return new JSNumber((int)(sbyte)this.buffer.buffer[this.byteOffset + index]);
                case TypedArrayType.Uint8Array:
                case TypedArrayType.Uint8ClampedArray:
                    return new JSNumber((int)this.buffer.buffer[this.byteOffset + index]);
                case TypedArrayType.Int16Array:
                    return new JSNumber((int)BitConverter.ToInt16(this.buffer.buffer, this.byteOffset + (int)index * 2));
                case TypedArrayType.Uint16Array:
                    return new JSNumber((int)BitConverter.ToUInt16(this.buffer.buffer, this.byteOffset + (int)index * 2));
                case TypedArrayType.Int32Array:
                    return new JSNumber(BitConverter.ToInt32(this.buffer.buffer, this.byteOffset + (int)index * 4));
                case TypedArrayType.Uint32Array:
                    return new JSNumber(BitConverter.ToUInt32(this.buffer.buffer, this.byteOffset + (int)index * 4));
                case TypedArrayType.Float32Array:
                    return new JSNumber((double)BitConverter.ToSingle(this.buffer.buffer, this.byteOffset + (int)index * 4));
                case TypedArrayType.Float64Array:
                    return new JSNumber(BitConverter.ToDouble(this.buffer.buffer, this.byteOffset + (int)index * 8));
                default:
                    if (throwError)
                    {
                        throw new NotSupportedException($"Unsupported TypedArray '{type}'.");
                    }
                    return JSUndefined.Value;
            }
        }

        internal override bool SetValue(uint index, JSValue value, JSValue receiver, bool throwError = true)
        {
            if (index < 0 || index >= this.length)
                return false;
            switch (type)
            {
                case TypedArrayType.Int8Array:
                    this.buffer.buffer[this.byteOffset + index] = (byte)value.IntValue;
                    break;

                case TypedArrayType.Uint8Array:
                    this.buffer.buffer[this.byteOffset + index] = (byte)(uint)value.IntValue;
                    break;

                case TypedArrayType.Uint8ClampedArray:

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
                    break;

                case TypedArrayType.Int16Array:
                    Array.Copy(BitConverter.GetBytes((Int16)value.IntValue), 0, this.buffer.buffer, this.byteOffset + index * 2, 2);
                    break;

                case TypedArrayType.Uint16Array:
                    Array.Copy(BitConverter.GetBytes((UInt16)value.IntValue), 0, this.buffer.buffer, this.byteOffset + index * 2, 2);
                    break;

                case TypedArrayType.Int32Array:
                    Array.Copy(BitConverter.GetBytes((Int32)value.IntValue), 0, this.buffer.buffer, this.byteOffset + index * 4, 4);
                    break;

                case TypedArrayType.Uint32Array:
                    Array.Copy(BitConverter.GetBytes((UInt32)value.IntValue), 0, this.buffer.buffer, this.byteOffset + index * 4, 4);
                    break;

                case TypedArrayType.Float32Array:
                    Array.Copy(BitConverter.GetBytes((float)value.DoubleValue), 0, this.buffer.buffer, this.byteOffset + index * 4, 4);
                    break;

                case TypedArrayType.Float64Array:
                    Array.Copy(BitConverter.GetBytes(value.DoubleValue), 0, this.buffer.buffer, this.byteOffset + index * 8, 8);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported TypedArray '{type}'.");
            }
            return true;
        }

        public override JSValue GetOwnPropertyDescriptor(JSValue name)
        {
            var key = name.ToKey(false);
            switch (key.Type)
            {
                case KeyType.String:
                    if (key.KeyString.Key == KeyStrings.length.Key)
                    {
                        var l = new JSObject();
                        l.FastAddValue(KeyStrings.value, new JSNumber(this.length), JSPropertyAttributes.ConfigurableValue);
                        l.FastAddValue(KeyStrings.writable, JSBoolean.False, JSPropertyAttributes.ConfigurableValue);
                        l.FastAddValue(KeyStrings.enumerable, JSBoolean.True, JSPropertyAttributes.ConfigurableValue);
                        return l;
                    }
                    break;
                case KeyType.UInt:
                    if (key.Index < (uint)this.length)
                    {
                        var l = new JSObject();
                        var v = GetValue(key.Index, this, false);
                        l.FastAddValue(KeyStrings.value, v, JSPropertyAttributes.ConfigurableValue);
                        l.FastAddValue(KeyStrings.writable, JSBoolean.True, JSPropertyAttributes.ConfigurableValue);
                        l.FastAddValue(KeyStrings.enumerable, JSBoolean.True, JSPropertyAttributes.ConfigurableValue);
                        l.FastAddValue(KeyStrings.configurable, JSBoolean.False, JSPropertyAttributes.ConfigurableValue);
                        return l;

                    }
                    return JSUndefined.Value;
            }
            return base.GetOwnPropertyDescriptor(name);
        }

        //public override JSValue this[uint index]
        //{
        //    get {
        //        if (index < 0 || index >= this.length)
        //            return JSUndefined.Value;
        //        switch (type)
        //        {
        //            case TypedArrayType.Int8Array:
        //                return new JSNumber((int)(sbyte)this.buffer.buffer[this.byteOffset + index]);
        //            case TypedArrayType.Uint8Array:
        //            case TypedArrayType.Uint8ClampedArray:
        //                return new JSNumber ((int)this.buffer.buffer[this.byteOffset + index]);
        //            case TypedArrayType.Int16Array:
        //                return new JSNumber ((int)BitConverter.ToInt16(this.buffer.buffer, this.byteOffset + (int)index * 2));
        //            case TypedArrayType.Uint16Array:
        //                return new JSNumber ((int)BitConverter.ToUInt16(this.buffer.buffer, this.byteOffset + (int) index * 2));
        //            case TypedArrayType.Int32Array:
        //                return new JSNumber (BitConverter.ToInt32(this.buffer.buffer, this.byteOffset + (int) index * 4));
        //            case TypedArrayType.Uint32Array:
        //                return new JSNumber (BitConverter.ToUInt32(this.buffer.buffer, this.byteOffset + (int) index * 4));
        //            case TypedArrayType.Float32Array:
        //                return new JSNumber ((double)BitConverter.ToSingle(this.buffer.buffer, this.byteOffset + (int) index * 4));
        //            case TypedArrayType.Float64Array:
        //                return new JSNumber (BitConverter.ToDouble(this.buffer.buffer, this.byteOffset + (int) index * 8));
        //            default:
        //                throw new NotSupportedException($"Unsupported TypedArray '{type}'.");
        //        }
        //    }
        //    set {
        //        if (index < 0 || index >= this.length)
        //            return;
        //        switch (type)
        //        {
        //            case TypedArrayType.Int8Array:
        //                this.buffer.buffer[this.byteOffset + index] = (byte)value.IntValue;
        //                break;

        //            case TypedArrayType.Uint8Array:
        //                this.buffer.buffer[this.byteOffset + index] = (byte)(uint)value.IntValue;
        //                break;

        //            case TypedArrayType.Uint8ClampedArray:

        //                // This algorithm is defined as ToUint8Clamp in the spec.
        //                double number = value.DoubleValue;
        //                int result;
        //                if (number <= 0)
        //                    result = 0;
        //                else if (number >= 255)
        //                    result = 255;
        //                else
        //                {
        //                    var f = Math.Floor(number);
        //                    if (f + 0.5 < number)
        //                        result = (int)f + 1;
        //                    else if (number < f + 0.5)
        //                        result = (int)f;
        //                    else if ((int)f % 2 == 0)
        //                        result = (int)f;
        //                    else
        //                        result = (int)f + 1;
        //                }
        //                this.buffer.buffer[this.byteOffset + index] = (byte)result;
        //                break;

        //            case TypedArrayType.Int16Array:
        //                Array.Copy(BitConverter.GetBytes((Int16)value.IntValue), 0, this.buffer.buffer, this.byteOffset + index * 2, 2);
        //                break;

        //            case TypedArrayType.Uint16Array:
        //                Array.Copy(BitConverter.GetBytes((UInt16)value.IntValue), 0, this.buffer.buffer, this.byteOffset + index * 2, 2);
        //                break;

        //            case TypedArrayType.Int32Array:
        //                Array.Copy(BitConverter.GetBytes((Int32)value.IntValue), 0, this.buffer.buffer, this.byteOffset + index * 4, 4);
        //                break;

        //            case TypedArrayType.Uint32Array:
        //                Array.Copy(BitConverter.GetBytes((UInt32)value.IntValue), 0, this.buffer.buffer, this.byteOffset + index * 4, 4);
        //                break;

        //            case TypedArrayType.Float32Array:
        //                Array.Copy(BitConverter.GetBytes((float)value.DoubleValue), 0, this.buffer.buffer, this.byteOffset + index * 4, 4);
        //                break;

        //            case TypedArrayType.Float64Array:
        //                Array.Copy(BitConverter.GetBytes(value.DoubleValue), 0, this.buffer.buffer, this.byteOffset + index * 8, 8);
        //                break;

        //            default:
        //                throw new NotSupportedException($"Unsupported TypedArray '{type}'.");
        //        }

        //    }
        //}

        public override bool BooleanValue => true;
        public override double DoubleValue => double.NaN;
        public override bool Equals(JSValue value)
        {
            return Object.ReferenceEquals(this, value);
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"{this} is not a function");
        }

        public override bool StrictEquals(JSValue value)
        {
            return Object.ReferenceEquals(this, value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                {
                    sb.Append(',');
                }

                sb.Append(this[(uint)i].ToString());
            }
            return sb.ToString();
        }

        public override string ToDetailString()
        {
            return this.ToString();
        }

        public override IElementEnumerator GetElementEnumerator()
        {
            return new ElementEnumerator(this);
        }

        internal IElementEnumerator GetElementEnumerator(int startIndex)
        {
            return new ElementEnumerator(this, startIndex);
        }

        internal IElementEnumerator GetEntries() {
            return new EntryEnumerator(this);
        }

        public override IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            return new KeyEnumerator(this.length);
        }

        internal JSGenerator GetKeys() {
            return new JSGenerator(new KeyEnumerator(this.length), "Array Iterator");
        }

        

        struct ElementEnumerator : IElementEnumerator
        {
            private TypedArray typedArray;
            private int index;

            public ElementEnumerator(TypedArray typedArray, int startIndex = 0)
            {
                this.typedArray = typedArray;
                this.index = startIndex - 1;
                
            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                if (++this.index < typedArray.length) {
                    hasValue = true;
                    index = (uint)this.index;
                    value = typedArray[index];
                    return true;
                }

                hasValue = false;
                index = 0;
                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNext(out JSValue value)
            {
                if (++this.index < typedArray.length)
                {
                    value = typedArray[(uint)index];
                    return true;
                }

                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNextOrDefault(out JSValue value, JSValue @default)
            {
                if (++this.index < typedArray.length)
                {
                    value = typedArray[(uint)index];
                    return true;
                }

                value = @default;
                return false;
            }

            public JSValue NextOrDefault(JSValue @default)
            {
                if (++this.index < typedArray.length)
                {
                    return typedArray[(uint)index];
                }

                return @default;
            }

        }

        struct EntryEnumerator : IElementEnumerator
        {
            private TypedArray typedArray;
            private int index;

            public EntryEnumerator(TypedArray typedArray)
            {
                this.typedArray = typedArray;
                this.index = -1;
            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                if (++this.index < typedArray.length)
                {
                    hasValue = true;
                    index = (uint)this.index;
                    value = new JSArray(new JSNumber(index), typedArray[index]);
                    return true;
                }

                hasValue = false;
                index = 0;
                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNext(out JSValue value)
            {
                if (++this.index < typedArray.length)
                {
                    value = new JSArray(new JSNumber(index),typedArray[(uint)index]);
                    return true;
                }

                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNextOrDefault(out JSValue value, JSValue @default)
            {
                if (++this.index < typedArray.length)
                {
                    value = new JSArray(new JSNumber(index), typedArray[(uint)index]);
                    return true;
                }

                value = @default;
                return false;
            }
            public JSValue NextOrDefault(JSValue @default)
            {
                if (++this.index < typedArray.length)
                {
                    return new JSArray(new JSNumber(index), typedArray[(uint)index]);
                }

                return @default;
            }
        }


    }

    internal struct KeyEnumerator : IElementEnumerator
    {
        private int length;
        private int index;

        public KeyEnumerator(int length)
        {
            this.length = length;
            this.index = -1;
        }

        public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
        {
            if (++this.index < this.length)
            {
                hasValue = true;
                index = (uint)this.index;
                value = new JSNumber(index);
                return true;
            }
            hasValue = false;
            index = 0;
            value = JSUndefined.Value;
            return false;
        }

        public bool MoveNext(out JSValue value)
        {
            if (++this.index < this.length)
            {
                value = new JSNumber(index);
                return true;
            }
            value = JSUndefined.Value;
            return false;
        }

        public bool MoveNextOrDefault(out JSValue value, JSValue @default)
        {
            if (++this.index < this.length)
            {
                value = new JSNumber(index);
                return true;
            }
            value = @default;
            return false;
        }

        public JSValue NextOrDefault(JSValue @default)
        {
            if (++this.index < this.length)
            {
                return new JSNumber(index);
            }
            return @default;
        }

    }
}
