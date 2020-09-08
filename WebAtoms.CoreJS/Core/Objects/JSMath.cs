using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core.Objects
{
    public class JSMath: JSObject
    {

        [Static("random")]
        public static JSValue Random(JSValue t, JSValue[] args)
        {
            return new JSNumber(1);
        }

    }
}
