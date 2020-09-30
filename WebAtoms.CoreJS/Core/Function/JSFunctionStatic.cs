using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSFunction
    {
        [Constructor]
        internal static JSValue Constructor(in Arguments args)
        {
            var len = args.Length;
            if (len == 0)
                throw new JSException("No arguments were supplied to Function constructor");
            JSValue body = null;
            var en = new Arguments.ArgumentsEnumerator();
            while (en.MoveNext())
            {

            }
            var sargs = args.All.Select(x =>
                {
                    body = x;
                    return x.ToString();
                })
                .ToList().Take(len-1).ToList();

            var bodyText = body is JSString @string ? @string.value : body.ToString();
            var fx = new JSFunction(JSFunction.empty, "internal", bodyText);


            // parse and create method...
            var fx1 = CoreScript.Compile(bodyText, "internal", sargs);
            fx.f = fx1;
            return fx;
        }

    }
}
