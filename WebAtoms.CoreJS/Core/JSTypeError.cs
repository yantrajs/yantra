using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSTypeError: JSError
    {

        public static readonly KeyString KeyTypeError = "TypeError";

        internal JSTypeError(JSValue message, JSValue stack) : base(message, stack, JSContext.Current.TypeErrorPrototype)
        {
        }

        public new static JSFunction Create()
        {
            var r = new JSFunction((t, a) => new JSTypeError(a[0], JSUndefined.Value));
            return r;
        }
    }
}
