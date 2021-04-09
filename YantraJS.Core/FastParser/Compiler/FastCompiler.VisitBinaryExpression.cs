using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;

namespace YantraJS.Core.FastParser.Compiler
{
    internal static class TokenTypesExtensions
    {
        public static Esprima.Ast.AssignmentOperator ToAssignmentOperator(this TokenTypes tokenType)
        {
            switch(tokenType) {
                case TokenTypes.Assign:
                    return Esprima.Ast.AssignmentOperator.Assign;
                case TokenTypes.AssignAdd:
                    return Esprima.Ast.AssignmentOperator.PlusAssign;
                case TokenTypes.AssignBitwideAnd:
                    return Esprima.Ast.AssignmentOperator.BitwiseAndAssign;
                case TokenTypes.AssignBitwideOr:
                    return Esprima.Ast.AssignmentOperator.BitwiseOrAssign;
                case TokenTypes.AssignDivide:
                    return Esprima.Ast.AssignmentOperator.DivideAssign;
                case TokenTypes.AssignLeftShift:
                    return Esprima.Ast.AssignmentOperator.LeftShiftAssign;
                case TokenTypes.AssignMod:
                    return Esprima.Ast.AssignmentOperator.ModuloAssign;
                case TokenTypes.AssignMultiply:
                    return Esprima.Ast.AssignmentOperator.TimesAssign;
                case TokenTypes.AssignPower:
                    return Esprima.Ast.AssignmentOperator.ExponentiationAssign;
                case TokenTypes.AssignRightShift:
                    return Esprima.Ast.AssignmentOperator.RightShiftAssign;
                case TokenTypes.AssignSubtract:
                    return Esprima.Ast.AssignmentOperator.MinusAssign;
                case TokenTypes.AssignUnsignedRightShift:
                    return Esprima.Ast.AssignmentOperator.UnsignedRightShiftAssign;
                case TokenTypes.AssignXor:
                    return Esprima.Ast.AssignmentOperator.BitwiseXOrAssign;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    partial class FastCompiler
    {
        protected override Expression VisitBinaryExpression(AstBinaryExpression binaryExpression)
        {
            switch (binaryExpression.Operator)
            {
                case TokenTypes.Assign:
                case TokenTypes.AssignAdd:
                case TokenTypes.AssignBitwideAnd:
                case TokenTypes.AssignBitwideOr:
                case TokenTypes.AssignDivide:
                case TokenTypes.AssignLeftShift:
                case TokenTypes.AssignMod:
                case TokenTypes.AssignMultiply:
                case TokenTypes.AssignPower:
                case TokenTypes.AssignRightShift:
                case TokenTypes.AssignSubtract:
                case TokenTypes.AssignUnsignedRightShift:
                case TokenTypes.AssignXor:
                    return VisitAssignmentExpression(binaryExpression.Left, binaryExpression.Operator.ToAssignmentOperator(), binaryExpression.Right);

                default:
                    throw new NotImplementedException();
            }
        }

    }
}
