using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public class JSStringStatic
    {

        [Static("fromCharCode")]
        internal static JSValue FromCharCode(in Arguments a)
        {
            StringBuilder sb = new StringBuilder();
            if (a.Length == 0)
                return new JSString(string.Empty);
            foreach(var ch in a.All)
            {
                sb.Append((char)ch.IntValue);
            }
            return new JSString(sb.ToString());
        }

    }
}
