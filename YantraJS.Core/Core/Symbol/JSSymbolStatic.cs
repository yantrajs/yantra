using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Runtime
{
    public static class JSSymbolStatic
    {

        [Prototype("iterator")]
        public static JSSymbol iterator = new JSSymbol("Symbol.iterator");

        [Prototype("toStringTag")]
        public static JSSymbol toStringTag = new JSSymbol("Symbol.toStringTag");

    }
}
