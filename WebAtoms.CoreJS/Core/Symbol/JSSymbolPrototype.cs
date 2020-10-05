using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core.Runtime
{
    public static class JSSymbolPrototype
    {

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            var name = a.Get1();
            if (name.IsUndefined)
                return new JSSymbol("");
            return new JSSymbol(name.ToString());
        }

    }
}
