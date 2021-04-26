#nullable enable
using System;

namespace YantraJS.Expressions
{
    public class YAssignExpression : YExpression
    {
        public readonly YExpression Left;
        public readonly YExpression Right;

        public YAssignExpression(YExpression left, YExpression right, Type? type)
            : base(YExpressionType.Assign, type ?? left.Type)
        {
            this.Left = left;
            this.Right = right;
        }
    }
}