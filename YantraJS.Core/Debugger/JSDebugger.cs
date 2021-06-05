using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Debugger
{
    public class JSDebugger
    {

        public static event EventHandler Break;

        public static object RaiseBreak()
        {
            Break?.Invoke(null, EventArgs.Empty);
            return null;
        }

    }
}
