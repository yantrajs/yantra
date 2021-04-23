#nullable enable
using System;

namespace YantraJS.Expressions
{
    public class YAssignExpression : YExpression
    {
        public readonly YExpression left;
        public readonly YExpression right;

        public YAssignExpression(YExpression left, YExpression right, Type? type)
            : base(YExpressionType.Assign, type ?? left.Type)
        {
            this.left = left;
            this.right = right;
        }
    }
}