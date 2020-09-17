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
        internal static JSValue FromCharCode(JSValue target, JSValue[] args)
        {
            StringBuilder sb = new StringBuilder();
            if (args == null)
                return new JSString(string.Empty);
            foreach(var ch in args)
            {
                sb.Append((char)ch.IntValue);
            }
            return new JSString(sb.ToString());
        }

    }
}
