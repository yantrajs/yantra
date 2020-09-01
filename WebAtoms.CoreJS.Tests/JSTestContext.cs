using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Tests.Core;

namespace WebAtoms.CoreJS.Tests
{


    public class JSTestContext: JSContext
    {

        private static KeyString assert = KeyStrings.GetOrCreate("assert");

        public JSTestContext()
        {
            var a = new JSFunction((t, a) => {
                var test = a[0];
                var message = a[1];
                message = message is JSUndefined ? new JSString("Assert failed, no message") : message;
                if (!JSBoolean.IsTrue(test))
                    throw new JSException(message);
                return JSUndefined.Value;
            });
            var prototype = Bootstrap.Create("Assert", typeof(JSAssert));
            a.prototypeChain = prototype.prototype;
            this[assert] = a;
        }

    }
}
