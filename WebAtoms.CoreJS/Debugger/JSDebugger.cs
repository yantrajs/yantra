using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Debugger
{
    public class JSDebugger
    {

        public static event EventHandler Break;

        public static void RaiseBreak()
        {
            Break?.Invoke(null, EventArgs.Empty);
        }

    }
}
