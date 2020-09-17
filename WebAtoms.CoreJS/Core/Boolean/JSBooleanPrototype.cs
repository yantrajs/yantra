using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSBooleanPrototype
    {

        [Constructor]
        public static JSValue Constructor(JSValue t, JSValue[] args)
        {
            var first = args.GetAt(0);
            return first.BooleanValue ? JSBooleanPrototype.True : JSBooleanPrototype.False;
        }

    }
}
