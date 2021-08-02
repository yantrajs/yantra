using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Debugger
{
    public abstract class JSDebugger
    {

        public static event EventHandler Break;

        public static object RaiseBreak()
        {
            Break?.Invoke(null, EventArgs.Empty);
            return null;
        }

        public abstract void ReportException(JSValue error);

        public abstract void ScriptParsed(string code, string codeFilePath);
    }
}
