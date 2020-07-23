using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Extensions
{
    public static class JSStringExtensions
    {

        public static JSString JSTrim(this JSString value)
        {
            return new JSString(value.value.JSTrim());
        }

        
        public static string JSTrim(this string text)
        {
            return text.Trim();
        }

        public static string JSTrim(this JSValue text)
        {
            switch (text) {
                case JSString jsString:
                    return JSTrim(jsString.value);
                case JSUndefined _:
                    return "undefined";
                case JSNull _:
                    return "null";
                case JSNumber n:
                    return n.value.ToString();
            }
            var txt = text.InvokeMethod("toString", JSArguments.Empty);
            return text.JSTrim();

        }
    }
}
