using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSBoolean
    {

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            var first = a.Get1();
            return first.BooleanValue ? JSBoolean.True : JSBoolean.False;
        }

    }
}
