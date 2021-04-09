using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using YantraJS.Utils;

namespace YantraJS.Core.FastParser.Compiler
{
    internal static class TokenTypesExtensions
    {
        public static (Esprima.Ast.BinaryOperator? binary, Esprima.Ast.AssignmentOperator? assign) 
            ToOperator(this TokenTypes tokenType)
        {
            switch (tokenType)
            {
                case TokenTypes.Assign:
                    return (null, Esprima.Ast.AssignmentOperator.Assign);
                case TokenTypes.AssignAdd:
                    return (null, Esprima.Ast.AssignmentOperator.PlusAssign);
                case TokenTypes.AssignBitwideAnd:
                    return (null, Esprima.Ast.AssignmentOperator.BitwiseAndAssign) ;
                case TokenTypes.AssignBitwideOr:
                    return (null, Esprima.Ast.AssignmentOperator.BitwiseOrAssign) ;
                case TokenTypes.AssignDivide:
                    return (null, Esprima.Ast.AssignmentOperator.DivideAssign);
                case TokenTypes.AssignLeftShift:
                    return (null, Esprima.Ast.AssignmentOperator.LeftShiftAssign);
                case TokenTypes.AssignMod:
                    return (null, Esprima.Ast.AssignmentOperator.ModuloAssign);
                case TokenTypes.AssignMultiply:
                    return (null, Esprima.Ast.AssignmentOperator.TimesAssign);
                case TokenTypes.AssignPower:
                    return (null, Esprima.Ast.AssignmentOperator.ExponentiationAssign);
                case TokenTypes.AssignRightShift:
                    return (null, Esprima.Ast.AssignmentOperator.RightShiftAssign);
                case TokenTypes.AssignSubtract:
                    return (null, Esprima.Ast.AssignmentOperator.MinusAssign);
                case TokenTypes.AssignUnsignedRightShift:
                    return (null, Esprima.Ast.AssignmentOperator.UnsignedRightShiftAssign);
                case TokenTypes.AssignXor:
                    return (null, Esprima.Ast.AssignmentOperator.BitwiseXOrAssign);

                case TokenTypes.Plus:
                    return (Esprima.Ast.BinaryOperator.Plus, null);
                case TokenTypes.Minus:
                    return (Esprima.Ast.BinaryOperator.Minus, null);
                case TokenTypes.Multiply:
                    return (Esprima.Ast.BinaryOperator.Times, null);
                case TokenTypes.Divide:
                    return (Esprima.Ast.BinaryOperator.Divide, null);
                case TokenTypes.Mod:
                    return (Esprima.Ast.BinaryOperator.Modulo, null);
                case TokenTypes.Equal:
                    return (Esprima.Ast.BinaryOperator.Equal, null);
                case TokenTypes.NotEqual:
                    return (Esprima.Ast.BinaryOperator.NotEqual, null);
                case TokenTypes.Greater:
                    return (Esprima.Ast.BinaryOperator.Greater, null);
                case TokenTypes.GreaterOrEqual:
                    return (Esprima.Ast.BinaryOperator.GreaterOrEqual, null);
                case TokenTypes.Less:
                    return (Esprima.Ast.BinaryOperator.Less, null);
                case TokenTypes.LessOrEqual:
                    return (Esprima.Ast.BinaryOperator.LessOrEqual, null);
                case TokenTypes.StrictlyEqual:
                    return (Esprima.Ast.BinaryOperator.StrictlyEqual, null);
                case TokenTypes.StrictlyNotEqual:
                    return (Esprima.Ast.BinaryOperator.StricltyNotEqual, null);
                case TokenTypes.BitwiseAnd:
                    return (Esprima.Ast.BinaryOperator.BitwiseAnd, null);
                case TokenTypes.BitwiseOr:
                    return (Esprima.Ast.BinaryOperator.BitwiseOr, null);
                case TokenTypes.Xor:
                    return (Esprima.Ast.BinaryOperator.BitwiseXOr, null);
                case TokenTypes.LeftShift:
                    return (Esprima.Ast.BinaryOperator.LeftShift, null);
                case TokenTypes.RightShift:
                    return (Esprima.Ast.BinaryOperator.RightShift, null);
                case TokenTypes.UnsignedRightShift:
                    return (Esprima.Ast.BinaryOperator.UnsignedRightShift, null);
                case TokenTypes.InstanceOf:
                    return (Esprima.Ast.BinaryOperator.InstanceOf, null);
                case TokenTypes.In:
                    return (Esprima.Ast.BinaryOperator.In, null);
                case TokenTypes.BooleanAnd:
                    return (Esprima.Ast.BinaryOperator.LogicalAnd, null);
                case TokenTypes.BooleanOr:
                    return (Esprima.Ast.BinaryOperator.LogicalOr, null);
                case TokenTypes.Power:
                    return (Esprima.Ast.BinaryOperator.Exponentiation, null);
            }

            return (null, null);

        }

    }

    partial class FastCompiler
    {
        protected override Expression VisitBinaryExpression(AstBinaryExpression binaryExpression)
        {
            var (binary, assign) = binaryExpression.Operator.ToOperator();
            if(assign != null)
                return VisitAssignmentExpression(
                    binaryExpression.Left, assign.Value, binaryExpression.Right);
            if(binary!=null)
            {
                var left = Visit(binaryExpression.Left);
                var right = Visit(binaryExpression.Right);
                if (binary.Value == Esprima.Ast.BinaryOperator.Plus)
                    return JSValueBuilder.Add(left, right);
                return BinaryOperation.Operation(left, right, binary.Value);
            }
            throw new NotImplementedException();

        }

    }
}
