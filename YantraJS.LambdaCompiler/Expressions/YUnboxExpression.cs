using System;
using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YUnboxExpression: YExpression
    {
        public readonly YExpression Target;

        public YUnboxExpression(YExpression target, Type type)
            : base(YExpressionType.Unbox, type)
        {
            this.Target = target;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write($"({Type.GetFriendlyName()})");
            Target.Print(writer);
        }
    }
}