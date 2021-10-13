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
        protected override CodeInfo VisitConditional(YConditionalExpression yConditionalExpression)
        {
            // Conditional statement must leave only one item on stack...

            // optimize for jumps...
            var test = yConditionalExpression.test;
            if (test.NodeType == YExpressionType.Binary && test is YBinaryExpression be)
            {
                if (TryVisitConditional(be.Left, be.Right, be.Operator, yConditionalExpression.@true, yConditionalExpression.@false)) {
                    return true;
                }
            }

            var trueEnd = il.DefineLabel("trueEnd", il.Top);
            var falseBegin = il.DefineLabel("falseBegin", il.Top);
            
            Visit(test);
            
            il.Emit(OpCodes.Brfalse, 
                yConditionalExpression.@false != null 
                ? falseBegin
                : trueEnd);
            
            Visit(yConditionalExpression.@true);

            if(yConditionalExpression.@false != null)
            {
                il.Emit(OpCodes.Br, trueEnd);
                il.MarkLabel(falseBegin);
                Visit(yConditionalExpression.@false);

                if(yConditionalExpression.@true.Type == typeof(void) 
                    && yConditionalExpression.@false.Type != typeof(void))
                {
                    il.Emit(OpCodes.Pop);
                }

                il.MarkLabel(trueEnd);
            }
            else
            {
                il.MarkLabel(trueEnd);
                //// we will need to leave something on stack..
                if (yConditionalExpression.@true.Type != typeof(void))
                {
                    il.Emit(OpCodes.Ldnull);
                }
            }

            return true;
        }

        private bool TryVisitConditional(
            YExpression left, 
            YExpression right, 
            YOperator @operator, 
            YExpression @true, 
            YExpression @false)
        {
            var type = left.Type;
            if (type != right.Type)
                return false;

            if (type != typeof(int) && type != typeof(uint))
                return false;

            var unsigned = type == typeof(uint);

            OpCode code;
            switch (@operator)
            {
                case YOperator.GreaterOrEqual:
                    code = unsigned ? OpCodes.Blt_Un : OpCodes.Blt;
                    break;
                case YOperator.Greater:
                    code = unsigned ? OpCodes.Ble_Un : OpCodes.Ble;
                    break;
                case YOperator.LessOrEqual:
                    code = unsigned ? OpCodes.Bgt_Un : OpCodes.Bgt;
                    break;
                case YOperator.Less:
                    code = unsigned ? OpCodes.Bge_Un : OpCodes.Bge;
                    break;
                case YOperator.Equal:
                    if (type == typeof(int))
                        return false;
                    code = OpCodes.Bne_Un;
                    break;
                case YOperator.NotEqual:
                    code = OpCodes.Beq;
                    break;
                default:
                    return false;
            }

            Visit(left);
            Visit(right);

            var trueEnd = il.DefineLabel("trueEnd", il.Top);
            var falseBegin = il.DefineLabel("falseBegin", il.Top);

            il.Emit(code,
                @false != null
                ? falseBegin
                : trueEnd);

            Visit(@true);

            if (@false != null)
            {
                if(@false.Type != typeof(void) && @true.Type == typeof(void))
                {
                    il.Emit(OpCodes.Ldnull);
                }

                il.Emit(OpCodes.Br, trueEnd);
                il.MarkLabel(falseBegin);
                Visit(@false);
                il.MarkLabel(trueEnd);
            }
            else
            {
                il.MarkLabel(trueEnd);
                if (@true.Type != typeof(void))
                {
                    il.Emit(OpCodes.Ldnull);
                }
            }

            return true;
        }
    }
}
