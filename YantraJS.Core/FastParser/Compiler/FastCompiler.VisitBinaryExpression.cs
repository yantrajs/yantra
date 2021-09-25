using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using YantraJS.Utils;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Expressions;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Expression VisitBinaryExpression(AstBinaryExpression binaryExpression)
        {

            var @operator = binaryExpression.Operator;

            if (@operator > TokenTypes.BeginAssignTokens && @operator < TokenTypes.EndAssignTokens)
                return VisitAssignmentExpression(
                    binaryExpression.Left, @operator, binaryExpression.Right);


            var (isLeftString, isLeftNumber, left) = ToNativeExpression(binaryExpression.Left);
            var (isRightString, isRightNumber, right) = ToNativeExpression(binaryExpression.Right);


            switch (@operator)
            {
                case TokenTypes.Plus:
                    if (isLeftNumber && isRightNumber)
                            return JSNumberBuilder.New( Expression.Add(left, right) );
                    
                    if (isLeftString && isRightString)
                    {
                        return JSStringBuilder.New(ClrStringBuilder.Concat(left, right));
                    }
                    if (isRightNumber)
                    {
                        return JSValueBuilder.AddDouble(ToJSValueExpression(left), right);
                    }
                    if (isRightString)
                    {
                        return JSValueBuilder.AddString(ToJSValueExpression(left), right);
                    }
                    return JSValueBuilder.Add(
                        ToJSValueExpression(left), 
                        ToJSValueExpression(right));
                case TokenTypes.Equal:
                    if (isLeftNumber)
                    {
                        // to do
                        // Add cocering...
                        if(isRightNumber)
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                Expression.Equal(left, right));
                    }
                    if (isLeftString)
                    {
                        if(isRightString)
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                ClrStringBuilder.Equal(left, right));
                    }
                    return JSValueBuilder.Equals(ToJSValueExpression(left), right);
                case TokenTypes.NotEqual:
                    if (isLeftNumber)
                    {
                        // to do
                        // Add cocering...
                        if (isRightNumber)
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                Expression.NotEqual(left, right));
                    }
                    if (isLeftString)
                    {
                        if (isRightString)
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                ClrStringBuilder.NotEqual(left, right));
                    }
                    return JSValueBuilder.NotEquals(ToJSValueExpression(left), right);

                case TokenTypes.StrictlyEqual:
                    if (isLeftNumber)
                    {
                        // to do
                        // Add cocering...
                        if (isRightNumber)
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                Expression.Equal(left, right));
                    }
                    if (isLeftString)
                    {
                        if (isRightString)
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                ClrStringBuilder.Equal(left, right));
                    }
                    return JSValueBuilder.StrictEquals(ToJSValueExpression(left), right);
                case TokenTypes.StrictlyNotEqual:
                    if (isLeftNumber)
                    {
                        // to do
                        // Add cocering...
                        if (isRightNumber)
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                Expression.NotEqual(left, right));
                    }
                    if (isLeftString)
                    {
                        if (isRightString)
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                ClrStringBuilder.NotEqual(left, right));
                    }
                    return JSValueBuilder.NotStrictEquals(ToJSValueExpression(left), right);
            }
            var be = BinaryOperation.Operation(
                ToJSValueExpression(left), 
                ToJSValueExpression(right), @operator);

            if (be == null)
                throw new FastParseException(binaryExpression.Start, $"Undefined binary operation {@operator}");

            return be;

        }

        public Expression ToJSValueExpression(Expression exp)
        {
            if (typeof(JSValue).IsAssignableFrom(exp.Type))
                return exp;
            if (exp.Type == typeof(string))
                return JSStringBuilder.New(exp);
            if (exp.Type == typeof(double))
                return JSNumberBuilder.New(exp);
            throw new NotImplementedException();
        }

        public (bool isString, bool isNumber, Expression exp) ToNativeExpression(AstExpression ast)
        {
            if(ast.Type == FastNodeType.Literal && ast is AstLiteral a) {
                switch (a.TokenType)
                {
                    case TokenTypes.String:
                        return (true, false,Expression.Constant(a.StringValue));
                    //case TokenTypes.True:
                    //    return Expression.Constant(true);
                    //case TokenTypes.False:
                    //    return Expression.Constant(false);
                    case TokenTypes.Number:
                        return (false, true, Expression.Constant(a.NumericValue));
                }
            }
            return (false, false, Visit(ast));
        }
    }
}
