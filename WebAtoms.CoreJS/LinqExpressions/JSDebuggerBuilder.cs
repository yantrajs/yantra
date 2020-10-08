using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSDebuggerBuilder : TypeHelper<Debugger.JSDebugger>
    {
        private static MethodInfo _RaiseBreak
            = Method("RaiseBreak");

        public static Expression RaiseBreak()
        {
            return Expression.Call(null, _RaiseBreak);
        }
    }
}
