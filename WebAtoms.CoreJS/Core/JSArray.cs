using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSArray: JSObject
    {
        public static JSObject CreatePrototype(JSContext context, JSObject obj)
        {
            var array = new JSObject();
            array.prototype = obj;
            context[KeyStrings.Array] = array;
            return array;
        }
    }
}
