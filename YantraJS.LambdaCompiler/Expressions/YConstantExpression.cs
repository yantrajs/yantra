using System;

namespace YantraJS.Expressions
{
    internal class YConstantExpression : YExpression
    {
        public readonly object Value;

        public YConstantExpression(object value, Type type)
            : base(YExpressionType.Constant, type)
        {
            this.Value = value;
        }
    }
}