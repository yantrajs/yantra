using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Debug
{
    public class JSConsole
    {

        private JSContext context;

        public JSConsole(JSContext context)
        {
            this.context = context;
        }

        public JSValue Log(in Arguments a)
        {
            var f = a.Get1();
            context.FireConsoleEvent("log", a);
            context.ReportLog(f);
            return f;
        }


        public JSValue Warn(in Arguments a)
        {
            var f = a.Get1();
            JSContext.Current.ReportLog(f);
            context.FireConsoleEvent("warn", a);
            return f;
        }

        public JSValue Error(in Arguments a)
        {
            var f = a.Get1();
            JSContext.Current.ReportLog(f);
            context.FireConsoleEvent("error", a);
            return f;
        }

    }
}
