using System;

namespace YantraJS.Expressions
{
    public class YNewArrayBoundsExpression: YExpression
    {
        public readonly Type ElementType;
        public readonly YExpression Size;

        public YNewArrayBoundsExpression(Type type, YExpression size)
            : base(YExpressionType.NewArrayBounds, type.MakeArrayType())
        {
            this.ElementType = type;
            this.Size = size;
        }
    }
}