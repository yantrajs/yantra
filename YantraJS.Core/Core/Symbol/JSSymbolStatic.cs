using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core
{
    public partial class JSSymbol
    {

        [JSExport("asyncDispose")]
        public static JSSymbol asyncDispose = new JSSymbol("@asyncDispose");


        [JSExport("dispose")]
        public static JSSymbol dispose = new JSSymbol("@dispose");

        [JSExport("asyncIterator")]
        public static JSSymbol asyncIterator = new JSSymbol("Symbol.asyncIterator");

        [JSExport("hasInstance")]
        public static JSSymbol hasInstance = new JSSymbol("Symbol.hasInstance");

        [JSExport("isConcatSpreadable")]
        public static JSSymbol isConcatSpreadable = new JSSymbol("Symbol.isConcatSpreadable");

        [JSExport("iterator")]
        public static JSSymbol iterator = new JSSymbol("Symbol.iterator");

        [JSExport("match")]
        public static JSSymbol match = new JSSymbol("Symbol.matchAll");

        [JSExport("replace")]
        public static JSSymbol replace = new JSSymbol("Symbol.replace");

        [JSExport("search")]
        public static JSSymbol search = new JSSymbol("Symbol.search");

        [JSExport("species")]
        public static JSSymbol species = new JSSymbol("Symbol.species");

        [JSExport("split")]
        public static JSSymbol split = new JSSymbol("Symbol.split");

        [JSExport("toPrimitive")]
        public static JSSymbol toPrimitive = new JSSymbol("Symbol.toPrimitive");

        [JSExport("toStringTag")]
        public static JSSymbol toStringTag = new JSSymbol("Symbol.toStringTag");

        [JSExport("unscopables")]
        public static JSSymbol unscopables = new JSSymbol("Symbol.unscopables");

        private static ConcurrentStringMap<JSSymbol> globals
            = ConcurrentStringMap<JSSymbol>.Create();

        public static JSSymbol GlobalSymbol(string name)
        {
            name = name.TrimStart('@');
            var f = typeof(JSSymbol).GetField(name);
            return (JSSymbol)f.GetValue(null);
        }

        [JSExport("for")]
        public static JSValue For(in Arguments a)
        {
            var name = a.Get1().ToString();
            return globals.GetOrCreate(name, (x) => new JSSymbol(x.Value));
        }
    }
}
