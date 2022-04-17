using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Expression VisitConditionalExpression(AstConditionalExpression conditionalExpression)
        {
            Exp EvaluateTest(AstExpression exp)
            {
                
                if (exp.IsUnaryExpression(out var u) && u.Operator == UnaryOperator.Negate)
                {
                    var eu = VisitExpression(u.Argument);
                    var e1 = JSValueBuilder.BooleanValue(eu);
                    var e2 = Exp.Not(e1);
                    return e2;
                }
                if (exp.IsBinaryExpression(out var b)
                    && b.Left.IsUnaryExpression(out u) && u.Operator == UnaryOperator.@typeof
                    && b.Right.IsStringLiteral(out var value))
                {
                    switch(value)
                    {
                        case "undefined":
                            if (b.Operator == TokenTypes.Equal || b.Operator == TokenTypes.StrictlyEqual)
                            {
                                return Exp.Equal(VisitExpression(u.Argument), JSUndefinedBuilder.Value);
                            }
                            if (b.Operator == TokenTypes.NotEqual || b.Operator == TokenTypes.StrictlyNotEqual)
                            {
                                return Exp.NotEqual(VisitExpression(u.Argument), JSUndefinedBuilder.Value);
                            }
                            break;
                        case "number":
                            if (b.Operator == TokenTypes.Equal || b.Operator == TokenTypes.StrictlyEqual)
                            {
                                return JSValueBuilder.IsNumber(VisitExpression(u.Argument));
                            }
                            if (b.Operator == TokenTypes.NotEqual || b.Operator == TokenTypes.StrictlyNotEqual)
                            {
                                return Expression.Not(JSValueBuilder.IsNumber(VisitExpression(u.Argument)));
                            }
                            break;
                        case "string":
                            if (b.Operator == TokenTypes.Equal || b.Operator == TokenTypes.StrictlyEqual)
                            {
                                return JSValueBuilder.IsString(VisitExpression(u.Argument));
                            }
                            if (b.Operator == TokenTypes.NotEqual || b.Operator == TokenTypes.StrictlyNotEqual)
                            {
                                return Expression.Not(JSValueBuilder.IsString(VisitExpression(u.Argument)));
                            }
                            break;
                        case "function":
                            if (b.Operator == TokenTypes.Equal || b.Operator == TokenTypes.StrictlyEqual)
                            {
                                return JSValueBuilder.IsFunction(VisitExpression(u.Argument));
                            }
                            if (b.Operator == TokenTypes.NotEqual || b.Operator == TokenTypes.StrictlyNotEqual)
                            {
                                return Expression.Not(JSValueBuilder.IsFunction(VisitExpression(u.Argument)));
                            }
                            break;
                        case "object":
                            if (b.Operator == TokenTypes.Equal || b.Operator == TokenTypes.StrictlyEqual)
                            {
                                return JSValueBuilder.IsObjectType(VisitExpression(u.Argument));
                            }
                            if (b.Operator == TokenTypes.NotEqual || b.Operator == TokenTypes.StrictlyNotEqual)
                            {
                                return Expression.Not(JSValueBuilder.IsObjectType(VisitExpression(u.Argument)));
                            }
                            break;
                        case "symbol":
                            if (b.Operator == TokenTypes.Equal || b.Operator == TokenTypes.StrictlyEqual)
                            {
                                return JSValueBuilder.IsSymbol(VisitExpression(u.Argument));
                            }
                            if (b.Operator == TokenTypes.NotEqual || b.Operator == TokenTypes.StrictlyNotEqual)
                            {
                                return Expression.Not(JSValueBuilder.IsSymbol(VisitExpression(u.Argument)));
                            }
                            break;
                    }
                }
                return JSValueBuilder.BooleanValue(VisitExpression(exp));
            }
            var test = EvaluateTest(conditionalExpression.Test);
            var @true = VisitExpression(conditionalExpression.True);
            var @false = VisitExpression(conditionalExpression.False);
            return Exp.Condition(
                test,
                @true,
                @false, typeof(JSValue));
        }
    }
}
