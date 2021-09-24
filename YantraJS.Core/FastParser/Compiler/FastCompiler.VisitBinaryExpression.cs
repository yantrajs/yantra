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
                    if(left.Type == typeof(double))
                    {
                        return JSValueBuilder.AddDouble(right, left);
                    }
                    if(right.Type == typeof(double))
                    {
                        return JSValueBuilder.AddDouble(left, right);
                    }

                    break;
            }

            // both literal should be fixed at parsing 

            if (@operator == TokenTypes.Plus)
            {
                if(r.Type == FastNodeType.Literal && r is AstLiteral literal)
                {
                    if (literal.TokenType == TokenTypes.Number) {
                        left = Visit(binaryExpression.Left);
                        return JSValueBuilder.AddDouble(left, Expression.Constant(literal.NumericValue));
                    }
                    if (literal.TokenType == TokenTypes.String)
                    {
                        left = Visit(binaryExpression.Left);
                        return JSValueBuilder.AddString(left, Expression.Constant(literal.StringValue));
                    }
                }

                if(l.Type == FastNodeType.Literal && l is AstLiteral leftLiteral)
                {
                    if(leftLiteral.TokenType == TokenTypes.Number)
                    {
                        right = Visit(binaryExpression.Right);
                        return JSValueBuilder.AddDouble(right, Expression.Constant(leftLiteral.NumericValue));
                    }
                    if (leftLiteral.TokenType == TokenTypes.String)
                    {
                        right = Visit(binaryExpression.Right);
                        return JSValueBuilder.AddString(right, Expression.Constant(leftLiteral.NumericValue));
                    }
                }

                left = Visit(binaryExpression.Left);
                right = Visit(binaryExpression.Right);
                return JSValueBuilder.Add(left, right);
            }


            left = Visit(binaryExpression.Left);
            right = Visit(binaryExpression.Right);
            var be = BinaryOperation.Operation(left, right, @operator);

            if (be == null)
                throw new FastParseException(binaryExpression.Start, $"Undefined binary operation {@operator}");

            return be;

        }

        public Expression ToDoubleExpression(Expression exp)
        {
            if (exp.Type == typeof(double))
                return exp;
            if (exp.Type == typeof(string))
                return Expression.Constant( double.Parse(((YConstantExpression)exp).Value.ToString()) );
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
                    case TokenTypes.True:
                        return Expression.Constant(true);
                    case TokenTypes.False:
                        return Expression.Constant(false);
                    case TokenTypes.Number:
                        return Expression.Constant(a.NumericValue);
                }
            }
            return Visit(ast);
        }
    }
}
