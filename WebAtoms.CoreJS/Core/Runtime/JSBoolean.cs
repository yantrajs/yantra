using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSBoolean
    {

        [Constructor]
        public static JSValue Constructor(JSValue t, JSValue[] args)
        {
            var first = args.GetAt(0);
            return first.BooleanValue ? JSContext.Current.True : JSContext.Current.False;
        }

    }
}
