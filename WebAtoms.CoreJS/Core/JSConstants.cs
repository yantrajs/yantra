using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    internal class JSConstants
    {

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
    }
}
