using System;
using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YBinaryExpression : YExpression
    {
        public readonly YExpression Left;
        public readonly YOperator Operator;
        public readonly YExpression Right;

        public YBinaryExpression(YExpression left, YOperator @operator, YExpression right)
            : base(YExpressionType.Binary, GetType(@operator, left.Type, right.Type))
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }

        private static Type GetType(YOperator @operator, Type leftType, Type rightType)
        {
            switch (@operator)
            {
                case YOperator.Less:
                case YOperator.LessOrEqual:
                case YOperator.Greater:
                case YOperator.GreaterOrEqual:
                case YOperator.Equal:
                case YOperator.NotEqual:
                    if(!leftType.IsAssignableFrom(rightType))
                    {
                        throw new NotSupportedException($"{@operator} cannot be applied {leftType} between {rightType}");
                    }
                    return typeof(bool);
            }
            return leftType;
        }

        public YExpression Update(YExpression left, YOperator @operator, YExpression right)
        {
            return new YBinaryExpression(left, @operator, right);
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("(");
            Left.Print(writer);
            writer.Write($" {ToString(Operator)} ");
            Right.Print(writer);
            writer.Write(")");
        }

        private string ToString(YOperator @operator)
        {
            switch (@operator)
            {
                case YOperator.Add:
                    return "+";
                case YOperator.Subtract:
                    return "-";
                case YOperator.Multipley:
                    return "*";
                case YOperator.Divide:
                    return "/";
                case YOperator.Mod:
                    return "%";
                case YOperator.Power:
                    return "**";
                case YOperator.Xor:
                    return "^";
                case YOperator.BitwiseAnd:
                    return "&";
                case YOperator.BitwiseOr:
                    return "|";
                case YOperator.BooleanAnd:
                    return "&&";
                case YOperator.BooleanOr:
                    return "||";
                case YOperator.Less:
                    return "<";
                case YOperator.LessOrEqual:
                    return "<=";
                case YOperator.Greater:
                    return ">";
                case YOperator.GreaterOrEqual:
                    return ">=";
                case YOperator.Equal:
                    return "==";
                case YOperator.NotEqual:
                    return "!=";
                case YOperator.LeftShift:
                    return "<<";
                case YOperator.RightShift:
                    return ">>";
                case YOperator.UnsignedRightShift:
                    return ">>>";
            }

            throw new NotImplementedException();
        }
    }
}