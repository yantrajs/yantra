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
        /// <summary>
        /// returns one if it is binary operator
        /// returns two if it is assignment operator
        /// returns zero if none
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="binary"></param>
        /// <param name="assign"></param>
        /// <returns></returns>
        public static int
            IsOperator(
                this TokenTypes tokenType, 
                out Esprima.Ast.BinaryOperator binary, 
                out Esprima.Ast.AssignmentOperator assign)
        {
            binary = default;
            assign = default;
            switch (tokenType)
            {
                case TokenTypes.Assign:
                    assign = Esprima.Ast.AssignmentOperator.Assign;
                    return 2;
                case TokenTypes.AssignAdd:
                    assign = Esprima.Ast.AssignmentOperator.PlusAssign;
                    return 2;
                case TokenTypes.AssignBitwideAnd:
                    assign = Esprima.Ast.AssignmentOperator.BitwiseAndAssign;
                    return 2;
                case TokenTypes.AssignBitwideOr:
                    assign = Esprima.Ast.AssignmentOperator.BitwiseOrAssign;
                    return 2;
                case TokenTypes.AssignDivide:
                    assign = Esprima.Ast.AssignmentOperator.DivideAssign;
                    return 2;
                case TokenTypes.AssignLeftShift:
                    assign = Esprima.Ast.AssignmentOperator.LeftShiftAssign;
                    return 2;
                case TokenTypes.AssignMod:
                    assign = Esprima.Ast.AssignmentOperator.ModuloAssign;
                    return 2;
                case TokenTypes.AssignMultiply:
                    assign = Esprima.Ast.AssignmentOperator.TimesAssign;
                    return 2;
                case TokenTypes.AssignPower:
                    assign = Esprima.Ast.AssignmentOperator.ExponentiationAssign;
                    return 2;
                case TokenTypes.AssignRightShift:
                    assign = Esprima.Ast.AssignmentOperator.RightShiftAssign;
                    return 2;
                case TokenTypes.AssignSubtract:
                    assign = Esprima.Ast.AssignmentOperator.MinusAssign;
                    return 2;
                case TokenTypes.AssignUnsignedRightShift:
                    assign = Esprima.Ast.AssignmentOperator.UnsignedRightShiftAssign;
                    return 2;
                case TokenTypes.AssignXor:
                    assign = Esprima.Ast.AssignmentOperator.BitwiseXOrAssign;
                    return 2;

                case TokenTypes.Plus:
                    binary = Esprima.Ast.BinaryOperator.Plus;
					return 1;
                case TokenTypes.Minus:
                    binary = Esprima.Ast.BinaryOperator.Minus;
					return 1;
                case TokenTypes.Multiply:
                    binary = Esprima.Ast.BinaryOperator.Times;
					return 1;
                case TokenTypes.Divide:
                    binary = Esprima.Ast.BinaryOperator.Divide;
					return 1;
                case TokenTypes.Mod:
                    binary = Esprima.Ast.BinaryOperator.Modulo;
					return 1;
                case TokenTypes.Equal:
                    binary = Esprima.Ast.BinaryOperator.Equal;
					return 1;
                case TokenTypes.NotEqual:
                    binary = Esprima.Ast.BinaryOperator.NotEqual;
					return 1;
                case TokenTypes.Greater:
                    binary = Esprima.Ast.BinaryOperator.Greater;
					return 1;
                case TokenTypes.GreaterOrEqual:
                    binary = Esprima.Ast.BinaryOperator.GreaterOrEqual;
					return 1;
                case TokenTypes.Less:
                    binary = Esprima.Ast.BinaryOperator.Less;
					return 1;
                case TokenTypes.LessOrEqual:
                    binary = Esprima.Ast.BinaryOperator.LessOrEqual;
					return 1;
                case TokenTypes.StrictlyEqual:
                    binary = Esprima.Ast.BinaryOperator.StrictlyEqual;
					return 1;
                case TokenTypes.StrictlyNotEqual:
                    binary = Esprima.Ast.BinaryOperator.StricltyNotEqual;
					return 1;
                case TokenTypes.BitwiseAnd:
                    binary = Esprima.Ast.BinaryOperator.BitwiseAnd;
					return 1;
                case TokenTypes.BitwiseOr:
                    binary = Esprima.Ast.BinaryOperator.BitwiseOr;
					return 1;
                case TokenTypes.Xor:
                    binary = Esprima.Ast.BinaryOperator.BitwiseXOr;
					return 1;
                case TokenTypes.LeftShift:
                    binary = Esprima.Ast.BinaryOperator.LeftShift;
					return 1;
                case TokenTypes.RightShift:
                    binary = Esprima.Ast.BinaryOperator.RightShift;
					return 1;
                case TokenTypes.UnsignedRightShift:
                    binary = Esprima.Ast.BinaryOperator.UnsignedRightShift;
					return 1;
                case TokenTypes.InstanceOf:
                    binary = Esprima.Ast.BinaryOperator.InstanceOf;
					return 1;
                case TokenTypes.In:
                    binary = Esprima.Ast.BinaryOperator.In;
					return 1;
                case TokenTypes.BooleanAnd:
                    binary = Esprima.Ast.BinaryOperator.LogicalAnd;
					return 1;
                case TokenTypes.BooleanOr:
                    binary = Esprima.Ast.BinaryOperator.LogicalOr;
					return 1;
                case TokenTypes.Power:
                    binary = Esprima.Ast.BinaryOperator.Exponentiation;
					return 1;
            }

            return 0;
        }


        //public static (Esprima.Ast.BinaryOperator? binary, Esprima.Ast.AssignmentOperator? assign) 
        //    ToOperator(this TokenTypes tokenType)
        //{
        //    switch (tokenType)
        //    {
        //        case TokenTypes.Assign:
        //            return (null, Esprima.Ast.AssignmentOperator.Assign);
        //        case TokenTypes.AssignAdd:
        //            return (null, Esprima.Ast.AssignmentOperator.PlusAssign);
        //        case TokenTypes.AssignBitwideAnd:
        //            return (null, Esprima.Ast.AssignmentOperator.BitwiseAndAssign) ;
        //        case TokenTypes.AssignBitwideOr:
        //            return (null, Esprima.Ast.AssignmentOperator.BitwiseOrAssign) ;
        //        case TokenTypes.AssignDivide:
        //            return (null, Esprima.Ast.AssignmentOperator.DivideAssign);
        //        case TokenTypes.AssignLeftShift:
        //            return (null, Esprima.Ast.AssignmentOperator.LeftShiftAssign);
        //        case TokenTypes.AssignMod:
        //            return (null, Esprima.Ast.AssignmentOperator.ModuloAssign);
        //        case TokenTypes.AssignMultiply:
        //            return (null, Esprima.Ast.AssignmentOperator.TimesAssign);
        //        case TokenTypes.AssignPower:
        //            return (null, Esprima.Ast.AssignmentOperator.ExponentiationAssign);
        //        case TokenTypes.AssignRightShift:
        //            return (null, Esprima.Ast.AssignmentOperator.RightShiftAssign);
        //        case TokenTypes.AssignSubtract:
        //            return (null, Esprima.Ast.AssignmentOperator.MinusAssign);
        //        case TokenTypes.AssignUnsignedRightShift:
        //            return (null, Esprima.Ast.AssignmentOperator.UnsignedRightShiftAssign);
        //        case TokenTypes.AssignXor:
        //            return (null, Esprima.Ast.AssignmentOperator.BitwiseXOrAssign);

        //        case TokenTypes.Plus:
        //            return (Esprima.Ast.BinaryOperator.Plus, null);
        //        case TokenTypes.Minus:
        //            return (Esprima.Ast.BinaryOperator.Minus, null);
        //        case TokenTypes.Multiply:
        //            return (Esprima.Ast.BinaryOperator.Times, null);
        //        case TokenTypes.Divide:
        //            return (Esprima.Ast.BinaryOperator.Divide, null);
        //        case TokenTypes.Mod:
        //            return (Esprima.Ast.BinaryOperator.Modulo, null);
        //        case TokenTypes.Equal:
        //            return (Esprima.Ast.BinaryOperator.Equal, null);
        //        case TokenTypes.NotEqual:
        //            return (Esprima.Ast.BinaryOperator.NotEqual, null);
        //        case TokenTypes.Greater:
        //            return (Esprima.Ast.BinaryOperator.Greater, null);
        //        case TokenTypes.GreaterOrEqual:
        //            return (Esprima.Ast.BinaryOperator.GreaterOrEqual, null);
        //        case TokenTypes.Less:
        //            return (Esprima.Ast.BinaryOperator.Less, null);
        //        case TokenTypes.LessOrEqual:
        //            return (Esprima.Ast.BinaryOperator.LessOrEqual, null);
        //        case TokenTypes.StrictlyEqual:
        //            return (Esprima.Ast.BinaryOperator.StrictlyEqual, null);
        //        case TokenTypes.StrictlyNotEqual:
        //            return (Esprima.Ast.BinaryOperator.StricltyNotEqual, null);
        //        case TokenTypes.BitwiseAnd:
        //            return (Esprima.Ast.BinaryOperator.BitwiseAnd, null);
        //        case TokenTypes.BitwiseOr:
        //            return (Esprima.Ast.BinaryOperator.BitwiseOr, null);
        //        case TokenTypes.Xor:
        //            return (Esprima.Ast.BinaryOperator.BitwiseXOr, null);
        //        case TokenTypes.LeftShift:
        //            return (Esprima.Ast.BinaryOperator.LeftShift, null);
        //        case TokenTypes.RightShift:
        //            return (Esprima.Ast.BinaryOperator.RightShift, null);
        //        case TokenTypes.UnsignedRightShift:
        //            return (Esprima.Ast.BinaryOperator.UnsignedRightShift, null);
        //        case TokenTypes.InstanceOf:
        //            return (Esprima.Ast.BinaryOperator.InstanceOf, null);
        //        case TokenTypes.In:
        //            return (Esprima.Ast.BinaryOperator.In, null);
        //        case TokenTypes.BooleanAnd:
        //            return (Esprima.Ast.BinaryOperator.LogicalAnd, null);
        //        case TokenTypes.BooleanOr:
        //            return (Esprima.Ast.BinaryOperator.LogicalOr, null);
        //        case TokenTypes.Power:
        //            return (Esprima.Ast.BinaryOperator.Exponentiation, null);
        //    }

        //    return (null, null);

        //}

    }

    partial class FastCompiler
    {
        protected override Expression VisitBinaryExpression(AstBinaryExpression binaryExpression)
        {
            int type = binaryExpression.Operator.IsOperator(out  var binary, out  var assign);
            if(type == 2)
                return VisitAssignmentExpression(
                    binaryExpression.Left, assign, binaryExpression.Right);
            if(type == 1)
            {
                var left = Visit(binaryExpression.Left);
                var right = Visit(binaryExpression.Right);
                if (binary == Esprima.Ast.BinaryOperator.Plus)
                    return JSValueBuilder.Add(left, right);
                return BinaryOperation.Operation(left, right, binary);
            }
            throw new NotImplementedException();

        }

    }
}
