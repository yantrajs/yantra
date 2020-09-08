using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSBoolean
    {

        [Constructor]
        public static JSValue Constructor(JSValue t, JSValue[] args)
        {
            return JSBoolean.IsTrue(t) ? JSContext.Current.True : JSContext.Current.False;
        }

    }
}
