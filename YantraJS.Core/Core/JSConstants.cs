using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{
    internal class JSConstants
    {
        public static readonly JSString Decimal =
            new JSString("decimal");

        public static readonly JSString Arguments =
            new JSString("arguments");

        public static readonly JSString BigInt =
            new JSString("bigint");

        public static readonly JSString Undefined =
            new JSString("undefined");

        public static readonly JSString Boolean =
            new JSString("boolean");

        public static readonly JSString String =
            new JSString("string");

        public static readonly JSString Object =
            new JSString("object");

        public static readonly JSString Number =
            new JSString("number");

        public static readonly JSString Function =
            new JSString("function");

        public static readonly JSString Symbol =
            new JSString("symbol");

        public static readonly JSString Infinity =
            new JSString("Infinity");
        public static readonly JSString NegativeInfinity =
            new JSString("-Infinity");
    }
}
