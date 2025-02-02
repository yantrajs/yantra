using System;
using Yantra.Core;

namespace YantraJS.Core.Typed
{
    //public abstract class TypedArray<T> : JSValue
    //{
    //    private readonly int length;
    //    private readonly T[] value;

    //    public TypedArray(int length, JSObject prototype) : base(prototype)
    //    {
    //        this.length = length;
    //        this.value = new T[length];
    //    }

    //    public override JSValue this[uint key] { 
    //        get => base[key]; 
    //        set => base[key] = value; 
    //    }
    //}

    [JSClassGenerator("ArrayBuffer")]
    public partial class JSArrayBuffer : JSObject
    {
        internal readonly byte[] buffer;

        public byte[] Buffer => buffer;

        // public override int Length { get => buffer.Length; set => throw new NotSupportedException(); }

        public JSArrayBuffer(in Arguments a): this(JSContext.NewTargetPrototype)
        {
            int length = a.Get1().AsInt32OrDefault();
            if (length < 0 || length > JSNumber.MaxSafeInteger)
            {
                throw JSContext.Current.NewRangeError("Buffer length out of range");
            }
            this.buffer = new byte[length];
        }

        public JSArrayBuffer(int length) : this()
        {
            this.buffer = new byte[length];
        }
        public JSArrayBuffer(byte[] buffer) : this()
        {
            this.buffer = buffer;
        }

        public override bool BooleanValue => true;

        public override double DoubleValue => Double.NaN;

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

    }
}
