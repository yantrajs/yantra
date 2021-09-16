using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core.Runtime
{
    public static class JSSymbolStatic
    {

        [Prototype("asyncIterator")]
        public static JSSymbol asyncIterator = new JSSymbol("Symbol.asyncIterator");

        [Prototype("hasInstance")]
        public static JSSymbol hasInstance = new JSSymbol("Symbol.hasInstance");

        [Prototype("isConcatSpreadable")]
        public static JSSymbol isConcatSpreadable = new JSSymbol("Symbol.isConcatSpreadable");

        [Prototype("iterator")]
        public static JSSymbol iterator = new JSSymbol("Symbol.iterator");

        [Prototype("match")]
        public static JSSymbol match = new JSSymbol("Symbol.matchAll");

        [Prototype("replace")]
        public static JSSymbol replace = new JSSymbol("Symbol.replace");

        [Prototype("search")]
        public static JSSymbol search = new JSSymbol("Symbol.search");

        [Prototype("species")]
        public static JSSymbol species = new JSSymbol("Symbol.species");

        [Prototype("split")]
        public static JSSymbol split = new JSSymbol("Symbol.split");

        [Prototype("toPrimitive")]
        public static JSSymbol toPrimitive = new JSSymbol("Symbol.toPrimitive");

        [Prototype("toStringTag")]
        public static JSSymbol toStringTag = new JSSymbol("Symbol.toStringTag");

        [Prototype("unscopables")]
        public static JSSymbol unscopables = new JSSymbol("Symbol.unscopables");

        private static ConcurrentStringMap<JSSymbol> globals
            = ConcurrentStringMap<JSSymbol>.Create();

        public static JSSymbol GlobalSymbol(string name)
        {
            name = name.TrimStart('@');
            var f = typeof(JSSymbolStatic).GetField(name);
            return (JSSymbol)f.GetValue(null);
        }

        [Static("for")]
        public static JSValue For(in Arguments a)
        {
            var name = a.Get1().ToString();
            return globals.GetOrCreate(name, (x) => new JSSymbol(x.Value));
        }
    }
}
