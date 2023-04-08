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

        public TypedArrayParameters(int length, int bytesPerElements)
        {
            this.length = length;
            this.byteOffset = 0;
            this.bytesPerElement = bytesPerElements;
        }

        public TypedArrayParameters(JSValue source, JSValue map, JSValue thisArg, int bytesPerElements)
        {
            this.length = -1;
            this.bytesPerElement = bytesPerElements;
            this.byteOffset = 0;
            this.copyFrom = source;
            this.map = map;
            this.thisArg = thisArg;
        }

        public TypedArrayParameters(
            in Arguments a, int bytesPerElements)
        {
            this.bytesPerElement = bytesPerElements;
            length = -1;
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