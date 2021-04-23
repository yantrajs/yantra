using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YantraJS.Core.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class JSTemplateArray
    {
        public static JSValue New(string[] cooked, string[] raw, JSValue[] arguments) {
            var args = new JSArray((uint)(arguments.Length + 1));
            var cookedArray = new JSArray((uint)cooked.Length);
            var rawArray = new JSArray((uint)cooked.Length);
            for (int i = 0; i < cooked.Length; i++)
            {
                cookedArray.Add(new JSString(cooked[i]));
            }
            for (int i = 0; i < raw.Length; i++)
            {
                rawArray.Add(new JSString(raw[i]));
            }

            cookedArray[KeyStrings.raw] = rawArray;

            args.Add(cookedArray);

            for (int i = 0; i < arguments.Length; i++)
            {
                args.Add(arguments[i]);
            }

            return args;
        }

    }
}
