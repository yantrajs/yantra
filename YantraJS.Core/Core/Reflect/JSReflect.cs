using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Internal
{
    public class JSReflectStatic
    {

        [Static("apply")]
        public static JSValue Apply(in Arguments a)
        {
            if(!(a[0] is JSFunction fx))
            {
                throw JSContext.Current.NewTypeError("Function expected");
            }
            return JSUndefined.Value;
        }

    }
}
