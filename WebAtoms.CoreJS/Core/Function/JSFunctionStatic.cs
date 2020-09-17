using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSFunctionStatic
    {
        [Constructor]
        internal static JSValue Constructor(JSValue t, JSValue[] args)
        {
            var len = args.Length;
            if (len == 0)
                throw new JSException("No arguments were supplied to Function constructor");
            var body = args[len - 1];
            var bodyText = body is JSString @string ? @string.value : body.ToString();
            var fx = new JSFunctionStatic(JSFunctionStatic.empty, "internal", bodyText);

            var sargs = args.Take(len - 1)
                .Select(x => x.ToString())
                .ToArray();

            // parse and create method...
            var fx1 = CoreScript.Compile(bodyText, "internal", sargs);
            fx.f = fx1;
            return fx;
        }

    }
}
