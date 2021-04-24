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

        private static Type GetType(YOperator @operator, Type left, Type right)
        {
            switch (@operator)
            {
                case YOperator.TypeAs:
                    return right;
                case YOperator.TypeIs:
                    return typeof(bool);
                case YOperator.Coalesc:
                    return right;
            }

            return left;
        }
    }
}