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


namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Expression VisitBinaryExpression(AstBinaryExpression binaryExpression)
        {

            var @operator = binaryExpression.Operator;
            Expression left;
            Expression right;

            if (@operator == TokenTypes.Plus)
            {
                left = Visit(binaryExpression.Left);
                right = Visit(binaryExpression.Right);
                return JSValueBuilder.Add(left, right);
            }            
            
            if(@operator > TokenTypes.BeginAssignTokens && @operator < TokenTypes.EndAssignTokens)
                return VisitAssignmentExpression(
                    binaryExpression.Left, @operator, binaryExpression.Right);

            left = Visit(binaryExpression.Left);
            right = Visit(binaryExpression.Right);
            var be = BinaryOperation.Operation(left, right, @operator);

            if (be == null)
                throw new FastParseException(binaryExpression.Start, $"Undefined binary operation {@operator}");

            return be;

        }

    }
}
