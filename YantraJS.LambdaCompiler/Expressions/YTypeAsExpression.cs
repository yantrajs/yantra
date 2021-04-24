using System;

namespace YantraJS.Expressions
{
    public class YTypeAsExpression: YExpression
    {
        public readonly YExpression Target;

        public YTypeAsExpression(YExpression target, Type type)
            : base(YExpressionType.TypeAs, type)
        {
            this.Target = target;
        }
    }
}