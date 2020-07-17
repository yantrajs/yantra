using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSFunction : JSObject
    {

        internal JSFunctionDelegate f;
        internal JSFunction(JSFunctionDelegate f)
        {
            this.f = f;
        }

        public override JSValue InvokeFunction(JSValue thisValue, JSValue args)
        {
            return f(thisValue, args);
        }
        
    }
}
