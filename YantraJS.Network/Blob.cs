using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Typed;

namespace YantraJS.Network
{
    internal static class JSArrayBufferExtensions
    {
        public static byte[] ToBuffer(this JSValue value)
        {
            if(value is JSString @string)
            {
                return @string.Encode(System.Text.Encoding.UTF8);
            }

            if (value is JSArrayBuffer @buffer)
            {
                return buffer.Buffer;
            }

            if(value is Blob blob)
            {
                return blob.Buffer;
            }

            // DataView is pending...

            throw JSContext.CurrentContext.NewTypeError($"Failed to convert {value} to ArrayBuffer");
        }
    }

    [JSClassGenerator]
    public partial class Blob : JSObject
    {
        public readonly byte[] Buffer;

        public Blob(in Arguments a) : base(JSContext.NewTargetPrototype)
        {
            var array = a[0] ?? throw JSContext.CurrentContext.NewTypeError("array is required");
            if(a.TryGetAt(1, out var options))
            {
                this.Type = options.TryGetProperty(Names.type, out var p) ? p : new JSString(StringSpan.Empty);
            }

            // save to array... 
            this.Buffer = array.ToBuffer();
        }

        private Blob(byte[] buffer, JSValue type): this()
        {
            Buffer = buffer;
            Type = type;
        }

        [JSExportSameName]
        public readonly static int None = 1;

        [JSExport]
        public JSValue Type { get; }

        [JSExport]
        public JSValue Size => new JSNumber(Buffer.Length);

        [JSExport]
        public JSValue ArrayBuffer => new JSArrayBuffer(Buffer);

        [JSExport]
        public JSValue Slice(in Arguments a)
        {
            return Slice(a.GetIntAt(0, 0), a.GetIntAt(1, this.Buffer.Length), a[2] ?? Type);
        }

        [JSExport]
        public JSValue Text(in Arguments a)
        {
            return new JSPromise(Task.Run<JSValue>(() =>
                new JSString(System.Text.Encoding.UTF8.GetString(Buffer))));
        }

        [JSExport]
        public JSValue Stream(in Arguments a)
        {
            throw JSContext.CurrentContext.NewTypeError("Not supported yet");
        }

        private JSValue Slice(int offset, int length, JSValue type)
        {
            if(offset < 0)
            {
                offset = length + offset;
            }
            var buffer = new byte[length];
            Array.Copy(Buffer, offset, buffer, 0, length);
            return new Blob(buffer, type);
        }
    }
}
