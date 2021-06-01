using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Generator;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;

namespace YantraJS.ExpHelper
{
    public class JSGeneratorFunctionBuilder
    {
        private static Type type = typeof(JSGeneratorFunction);

        private static ConstructorInfo _New =
            type.Constructor(
                typeof(ScriptInfo),
                typeof(JSVariable[]), typeof(JSGeneratorDelegate), StringSpanBuilder.RefType, StringSpanBuilder.RefType);

        public static Expression New(Expression scriptInfo, Expression closures, Expression @delegate, Expression name, Expression code)
        {
            return Expression.New(_New, scriptInfo, closures, @delegate, name, code);
        }
    }
}
