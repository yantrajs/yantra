using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSRegExp: JSObject
    {

        readonly string pattern;
        readonly string flags;
        public JSRegExp(string pattern, string flags)
        {
            this.flags = flags;
            this.pattern = pattern;
        }

    }
}
