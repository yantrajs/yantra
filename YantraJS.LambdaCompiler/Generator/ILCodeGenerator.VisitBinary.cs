using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {
        protected override CodeInfo VisitBinary(YBinaryExpression yBinaryExpression)
        {
            switch (yBinaryExpression.Operator)
            {
                case YOperator.BooleanAnd:
                    {
                        var trueEnd = il.DefineLabel("trueEnd", il.Top);
                        var falseEnd = il.DefineLabel("falseEnd", il.Top);
                        Visit(yBinaryExpression.Left);
                        il.Emit(OpCodes.Brfalse, trueEnd);
                        Visit(yBinaryExpression.Right);
                        il.Emit(OpCodes.Br, falseEnd);
                        il.MarkLabel(trueEnd);
                        il.EmitConstant(0);
                        il.MarkLabel(falseEnd);
                    }
                    return true;
                case YOperator.BooleanOr:
                    {
                        var trueEnd = il.DefineLabel("trueEnd", il.Top);
                        var falseEnd = il.DefineLabel("falseEnd", il.Top);
                        Visit(yBinaryExpression.Left);
                        il.Emit(OpCodes.Brtrue, trueEnd);
                        Visit(yBinaryExpression.Right);
                        il.Emit(OpCodes.Br, falseEnd);
                        il.MarkLabel(trueEnd);
                        il.EmitConstant(1);
                        il.MarkLabel(falseEnd);
                    }
                    return true;
            }


            Visit(yBinaryExpression.Left);
            Visit(yBinaryExpression.Right);
            switch (yBinaryExpression.Operator)
            {
                case YOperator.Add:
                    il.Emit(OpCodes.Add);
                    break;
                case YOperator.Subtract:
                    il.Emit(OpCodes.Sub);
                    break;
                case YOperator.Multipley:
                    il.Emit(OpCodes.Mul);
                    break;
                case YOperator.Divide:
                    il.Emit(OpCodes.Div);
                    break;
                case YOperator.Mod:
                    il.Emit(OpCodes.Rem);
                    break;
                case YOperator.Xor:
                    il.Emit(OpCodes.Xor);
                    break;
                case YOperator.BitwiseAnd:
                    il.Emit(OpCodes.And);
                    break;
                case YOperator.BitwiseOr:
                    il.Emit(OpCodes.Or);
                    break;
                case YOperator.Less:
                    il.Emit(OpCodes.Clt);
                    break;
                case YOperator.LessOrEqual:
                    il.Emit(OpCodes.Cgt);
                    il.EmitConstant(0);
                    il.Emit(OpCodes.Ceq);
                    break;
                case YOperator.Greater:
                    il.Emit(OpCodes.Cgt);
                    break;
                case YOperator.GreaterOrEqual:
                    il.Emit(OpCodes.Clt);
                    il.EmitConstant(0);
                    il.Emit(OpCodes.Ceq);
                    break;
                case YOperator.Equal:
                    il.Emit(OpCodes.Ceq);
                    break;
                case YOperator.NotEqual:
                    il.Emit(OpCodes.Ceq);
                    il.EmitConstant(0);
                    il.Emit(OpCodes.Ceq);
                    break;
                case YOperator.LeftShift:
                    il.Emit(OpCodes.Shl);
                    break;
                case YOperator.RightShift:
                    il.Emit(OpCodes.Shr);
                    break;
                case YOperator.UnsignedRightShift:
                    il.Emit(OpCodes.Shr_Un);
                    break;
                default:
                    throw new NotSupportedException($"{yBinaryExpression.Operator}");
            }

            return true;
        }
    }
}
