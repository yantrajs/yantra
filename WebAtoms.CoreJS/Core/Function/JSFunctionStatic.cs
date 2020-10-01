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
            var al = args.Length;
            var last = al - 1;
            var sargs = new List<string>();
            for(var ai=0; ai<al; ai++)
            {
                var item = args.GetAt(ai);
                if (ai == last)
                {
                    body = item;
                } else
                {
                    sargs.Add(item.ToString());
                }
            }

            var bodyText = body is JSString @string ? @string.value : body.ToString();
            var fx = new JSFunction(JSFunction.empty, "internal", bodyText);


            // parse and create method...
            var fx1 = CoreScript.Compile(bodyText, "internal", sargs);
            fx.f = fx1;
            return fx;
        }

    }
}
