using System;
using System.CodeDom.Compiler;

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

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write($"new {ElementType.FullName} [");
            Size.Print(writer);
            writer.Write("]");
        }
    }
}