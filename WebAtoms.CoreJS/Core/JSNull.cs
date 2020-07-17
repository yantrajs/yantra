using System;
using System.Linq;

namespace WebAtoms.CoreJS.Core
{
    public sealed class JSNull : JSValue
    {
        private JSNull()
        {

        }

        public static JSNull Value = new JSNull();

        public override JSValue this[JSValue key] {
            get => throw new InvalidOperationException($"Cannot get {key} of null");
            set => throw new InvalidOperationException($"Cannot set {key} of null");
        }
    }
}
