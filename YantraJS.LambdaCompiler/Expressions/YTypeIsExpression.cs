using System;

namespace YantraJS.Expressions
{
    public class YTypeIsExpression: YExpression
    {
        public readonly YExpression Target;
        public readonly Type AsType;

        public YTypeIsExpression(YExpression target, Type type)
            : base(YExpressionType.TypeIs, typeof(bool))
        {
            this.Target = target;
            this.AsType = type;
        }
    }
}