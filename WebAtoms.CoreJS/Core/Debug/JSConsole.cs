using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core.Debug
{
    public class JSConsole: JSObject
    {

        [Prototype("log")]
        public static JSValue Log(in Arguments a)
        {
            var f = a.Get1();
            JSContext.Current.ReportLog(f);
            return f;
        }

    }
}
