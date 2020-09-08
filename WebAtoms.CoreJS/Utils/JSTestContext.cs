using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Utils
{


    public class JSTestContext: JSContext
    {

        private static KeyString assert = KeyStrings.GetOrCreate("assert");

        public JSTestContext()
        {
            var a = new JSFunction((t, a1) => {
                var test = a1.GetAt(0);
                var message = a1.GetAt(1);
                message = message.IsUndefined ? new JSString("Assert failed, no message") : message;
                if (!test.BooleanValue)
                    throw new JSException(message);
                return JSUndefined.Value;
            });
            var prototype = Bootstrap.Create("Assert", typeof(JSAssert));
            a.prototypeChain = prototype.prototype;
            this[assert] = a;
        }

    }
}
