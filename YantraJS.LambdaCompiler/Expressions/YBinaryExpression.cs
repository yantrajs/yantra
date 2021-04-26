using System;

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
    }
}