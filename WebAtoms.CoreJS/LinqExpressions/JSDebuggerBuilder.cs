using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Debugger;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSDebuggerBuilder
    {
        private static Type type = typeof(JSDebugger);

        private static MethodInfo _RaiseBreak
            = type.InternalMethod(nameof(JSDebugger.RaiseBreak));

        public static Expression RaiseBreak()
        {
            return Expression.Call(null, _RaiseBreak);
        }
    }
}
