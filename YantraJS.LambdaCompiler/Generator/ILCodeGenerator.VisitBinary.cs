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
                case YOperator.BooleanAnd:
                    il.Emit(OpCodes.And);
                    break;
                case YOperator.BooleanOr:
                    il.Emit(OpCodes.Or);
                    break;
                case YOperator.Less:
                    il.Emit(OpCodes.Clt);
                    break;
                case YOperator.LessOrEqual:
                    il.Emit(OpCodes.Cgt);
                    break;
                case YOperator.Greater:
                    il.Emit(OpCodes.Cgt);
                    break;
                case YOperator.GreaterOrEqual:
                    il.Emit(OpCodes.Clt);
                    break;
                case YOperator.Equal:
                    il.Emit(OpCodes.Ceq);
                    break;
                case YOperator.NotEqual:
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Neg);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return true;
        }
    }
}
