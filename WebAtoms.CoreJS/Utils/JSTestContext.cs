using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Utils
{


    public class JSTestContext: JSContext
    {

        public JSTestContext()
        {
            var a = new JSFunction((in Arguments args) => {
                var (test, message) = args.Get2();
                message = message.IsUndefined ? new JSString($"Assert failed, no message") : message;
                if (!test.BooleanValue)
                    throw new JSException(message);
                return JSUndefined.Value;
            });
            var prototype = Bootstrap.Create("Assert", typeof(JSAssert));
            a.prototypeChain = prototype.prototype;
            this[KeyStrings.assert] = a;
        }

    }
}
