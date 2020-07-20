using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS
{
    public class ExpressionExecutor
    {

        public static JSValue Execute(Esprima.Ast.BinaryExpression exp, JSValue left, JSValue right)
        {
            using(CallStack.Current.Push(exp))
            {
                return JSUndefined.Value;
            }
        }

    }
}
