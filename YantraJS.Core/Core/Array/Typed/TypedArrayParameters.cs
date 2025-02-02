namespace YantraJS.Core.Typed
{
    public readonly struct TypedArrayParameters
    {
        public readonly JSArrayBuffer buffer;
        public readonly int length;
        public readonly int bytesPerElement;
        public readonly int byteOffset;
        public readonly JSValue copyFrom;
        public readonly JSValue map;
        public readonly JSValue thisArg;
        public readonly JSObject prototype;

        public static TypedArrayParameters From(in Arguments a, int bytesPerElements)
        {
            var (f, map, mapThis) = a.Get3();
            return new TypedArrayParameters(f, map, mapThis, bytesPerElements, (a.This as JSFunction).prototype);
        }

        public static TypedArrayParameters Of(in Arguments a, int bytesPerElements)
        {
            return new TypedArrayParameters(a.Length, bytesPerElements, (a.This as JSFunction).prototype);
        }

        private TypedArrayParameters(int length, int bytesPerElements, JSObject prototype)
        {
            buffer = null;
            this.length = length;
            this.bytesPerElement = bytesPerElements;
            this.byteOffset = 0;
            copyFrom = null;
            map = null;
            thisArg = null;
            this.prototype = prototype;
        }

        private TypedArrayParameters(JSValue source, JSValue map, JSValue thisArg, int bytesPerElements, JSObject prototype)
        {
            buffer = null;
            this.length = -1;
            this.bytesPerElement = bytesPerElements;
            this.byteOffset = 0;
            this.copyFrom = source;
            this.map = map;
            this.thisArg = thisArg;
            this.prototype = prototype;
        }

        public TypedArrayParameters(
            in Arguments a, int bytesPerElements)
        {
            buffer = null;
            length = -1;
            this.bytesPerElement = bytesPerElements;
            byteOffset = 0;
            copyFrom = null;
            map = null;
            thisArg = null;
            prototype = JSContext.NewTargetPrototype;
            if (a.Length == 0)
            {
                buffer = null;
                byteOffset = 0;
                length = 0;
                return;
            }
            var (a1, a2, a3) = a.Get3();
            if (a1.IsNumber)
            {
                buffer = null;
                byteOffset = 0;
                length = a1.IntValue;
                return;
            }
            if (a1 is JSArrayBuffer arrayBuffer)
            {
                buffer = arrayBuffer;
                byteOffset = a2.AsInt32OrDefault();
                length = a3.AsInt32OrDefault(arrayBuffer.Length);
                return;
            }
            this.copyFrom = a1;
        }
    }

}