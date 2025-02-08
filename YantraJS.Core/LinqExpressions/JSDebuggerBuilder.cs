using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Debugger;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Core.LambdaGen;

namespace YantraJS.ExpHelper
{
    public class JSDebuggerBuilder
    {
        //private static Type type = typeof(JSDebugger);

        //private static MethodInfo _RaiseBreak
        //    = type.InternalMethod(nameof(JSDebugger.RaiseBreak));

        public static Expression RaiseBreak()
        {
            return NewLambdaExpression.StaticCallExpression(() => () => JSDebugger.RaiseBreak());
            // return Expression.Call(null, _RaiseBreak);
        }
    }
}
