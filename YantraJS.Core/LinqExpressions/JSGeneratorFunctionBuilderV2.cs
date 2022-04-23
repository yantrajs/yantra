using System;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Generator;
using YantraJS.Core.LinqExpressions.GeneratorsV2;
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
    public class JSGeneratorFunctionBuilderV2
    {
        private static Type type = typeof(JSGeneratorFunctionV2);

        private static ConstructorInfo _New =
            type.Constructor(
                typeof(JSGeneratorDelegateV2), StringSpanBuilder.RefType, StringSpanBuilder.RefType);

        public static Expression New(Expression @delegate, Expression name, Expression code)
        {
            return Expression.New(_New, @delegate, name, code);
        }
    }
}
