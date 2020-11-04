using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{
    public class JSStringStatic
    {

        [Static("fromCharCode")]
        internal static JSValue FromCharCode(in Arguments a)
        {
            StringBuilder sb = new StringBuilder();
            if (a.Length == 0)
                return new JSString(string.Empty);
            var al = a.Length;
            for(var ai = 0; ai < al; ai++)
            {
                var ch = a.GetAt(ai);
                sb.Append((char)ch.IntValue);
            }
            return new JSString(sb.ToString());
        }

    }
}
