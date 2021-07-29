using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Debug
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


        [Prototype("warn")]
        public static JSValue Warn(in Arguments a)
        {
            var f = a.Get1();
            JSContext.Current.ReportLog(f);
            return f;
        }

        [Prototype("error")]
        public static JSValue Error(in Arguments a)
        {
            var f = a.Get1();
            JSContext.Current.ReportLog(f);
            return f;
        }

    }
}
