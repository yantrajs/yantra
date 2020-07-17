using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSObject : JSValue
    {

        public JSObject()
        {
            ownProperties = new BinaryUInt32Map<JSProperty>();
        }

        internal static JSProperty hasOwnProperty = new JSProperty
        {
            value = new JSFunction((t, a) =>
            {
                return JSUndefined.Value;
            })
        };

        internal static JSProperty toString = JSProperty.Function(
            (t, a) => new JSString(t.ToString()));

    }
}
