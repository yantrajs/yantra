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


            Expression left = ToNativeExpression(binaryExpression.Left);
            Expression right = ToNativeExpression(binaryExpression.Right);


            switch (@operator)
            {
                case TokenTypes.Plus:
                    if (left.Type == typeof(double) && right.Type == typeof(double))
                            return JSNumberBuilder.New( Expression.Add(left, right) );
                    
                    if (left.Type == typeof(string) && right.Type == typeof(string))
                    {
                        return JSStringBuilder.New(ClrStringBuilder.Concat(left, right));
                    }
                    if (right.Type == typeof(double))
                    {
                        return JSValueBuilder.AddDouble(ToJSValueExpression(left), right);
                    }
                    if (right.Type == typeof(string))
                    {
                        return JSValueBuilder.AddString(ToJSValueExpression(left), right);
                    }
                    return JSValueBuilder.Add(
                        ToJSValueExpression(left), 
                        ToJSValueExpression(right));
                case TokenTypes.Equal:
                    if (left.Type == typeof(double))
                    {
                        // to do
                        // Add cocering...
                        if(right.Type == typeof(double))
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                Expression.Equal(left, right));
                    }
                    if (left.Type == typeof(string))
                    {
                        if(right.Type == typeof(string))
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                ClrStringBuilder.Equal(left, right));
                    }
                    return JSValueBuilder.Equals(ToJSValueExpression(left), right);
                case TokenTypes.NotEqual:
                    if (left.Type == typeof(double))
                    {
                        // to do
                        // Add cocering...
                        if (right.Type == typeof(double))
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                Expression.NotEqual(left, right));
                    }
                    if (left.Type == typeof(string))
                    {
                        if (right.Type == typeof(string))
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                ClrStringBuilder.NotEqual(left, right));
                    }
                    return JSValueBuilder.NotEquals(ToJSValueExpression(left), right);

                case TokenTypes.StrictlyEqual:
                    if (left.Type == typeof(double))
                    {
                        // to do
                        // Add cocering...
                        if (right.Type == typeof(double))
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                Expression.Equal(left, right));
                    }
                    if (left.Type == typeof(string))
                    {
                        if (right.Type == typeof(string))
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                ClrStringBuilder.Equal(left, right));
                    }
                    return JSValueBuilder.StrictEquals(ToJSValueExpression(left), right);
                case TokenTypes.StrictlyNotEqual:
                    if (left.Type == typeof(double))
                    {
                        // to do
                        // Add cocering...
                        if (right.Type == typeof(double))
                            return JSBooleanBuilder.NewFromCLRBoolean(
                                Expression.NotEqual(left, right));
                    }
                    if (left.Type == typeof(string))
                    {
                        if (right.Type == typeof(string))
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

        public Expression ToDoubleExpression(Expression exp)
        {
            if (exp.Type == typeof(double))
                return exp;
            if (exp.Type == typeof(string) && exp is YConstantExpression ce)
            {
                var ds = ce.Value.ToString();
                return Expression.Constant(double.TryParse(ds, out var d)
                    ? d
                    : string.IsNullOrWhiteSpace(ds) 
                        ? 0.0
                        : 1);
            }
            return JSValueBuilder.DoubleValue(exp);
        }

        public Expression ToStringExpression(Expression exp)
        {
            if (exp.Type == typeof(string))
                return exp;
            if (exp.Type == typeof(double))
                return Expression.Constant(((YConstantExpression)exp).Value.ToString());
            return ObjectBuilder.ToString(exp);
        }


        public Expression ToNativeExpression(AstExpression ast)
        {
            if(ast.Type == FastNodeType.Literal && ast is AstLiteral a) {
                switch (a.TokenType)
                {
                    case TokenTypes.String:
                        return Expression.Constant(a.StringValue);
                    //case TokenTypes.True:
                    //    return Expression.Constant(true);
                    //case TokenTypes.False:
                    //    return Expression.Constant(false);
                    case TokenTypes.Number:
                        return Expression.Constant(a.NumericValue);
                }
            }
            return Visit(ast);
        }
    }
}
