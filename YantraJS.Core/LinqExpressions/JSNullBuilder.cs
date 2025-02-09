using System;
using System.Linq;
using System.Linq.Expressions;
using YantraJS.Core;

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
    public class JSNullBuilder
    {

        public static Expression Value =
             // Expression.TypeAs(
                 NewLambdaExpression.StaticFieldExpression<JSValue>(() => () => JSNull.Value)
                 //, Expression.Field(
                 //       null, 
                 //       typeof(JSNull)
                 //           .GetField(nameof(JSNull.Value))), 
                 // typeof(JSValue))
                 ;
    }
}
