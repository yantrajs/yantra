using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core.Array.Typed
{

    public class TypedArray : JSObject
    {
        private readonly JSArrayBuffer buffer;
        private readonly int byteOffset;
        private readonly int bytesPerElement;

        public TypedArray(JSArrayBuffer buffer, int byteOffset, int length, int bytesPerElement,  JSObject prototype) : base(prototype)
        {
            if (length == -1)
            {
                length = buffer.buffer.Length;
            }
            if (((length - byteOffset) % bytesPerElement) != 0)
            {
                throw JSContext.Current.NewRangeError($"byte length of {this.GetType().Name} should be multiple of {bytesPerElement}");
            }

            this.buffer = buffer;
            this.byteOffset = byteOffset;
            this.bytesPerElement = bytesPerElement;
        }

        public override JSValue this[uint key]
        {
            get {
                throw new NotImplementedException();
            }
            set => base[key] = value;
        }

        public override bool BooleanValue => true;
        public override double DoubleValue => double.NaN;
        public override JSBoolean Equals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"{this} is not a function");
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBoolean.True;
            return JSBoolean.False;
        }
       
    }
}
