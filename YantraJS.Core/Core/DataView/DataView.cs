using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Typed;

namespace YantraJS.Core.Core.DataView
{
    [JSRuntime(typeof(DataViewStatic), typeof(DataViewPrototype))]
    public class DataView : JSObject
    {
        internal readonly JSArrayBuffer buffer;
        internal readonly int byteOffset;
        internal readonly int byteLength;

      

    public DataView(
        JSArrayBuffer buffer,
        int byteOffset,
        int byteLength) : base(JSContext.Current.DataViewPrototype)
        
        {
            this.buffer = buffer;
            this.byteLength = byteLength;
            this.byteOffset = byteOffset;
        }

    }
}
