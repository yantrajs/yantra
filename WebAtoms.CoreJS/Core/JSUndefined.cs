using System;
using System.Linq;

namespace WebAtoms.CoreJS.Core
{
    public sealed class JSUndefined : JSValue
    {
        private JSUndefined()
        {

        }

        public static JSUndefined Value = new JSUndefined();

        public override JSValue this[JSValue key]
        {
            get => throw new InvalidOperationException($"Cannot get {key} of undefined");
            set => throw new InvalidOperationException($"Cannot set {key} of undefined");
        }

    }
}
