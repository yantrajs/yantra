using System;
using System.CodeDom.Compiler;

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

        public override void Print(IndentedTextWriter writer)
        {
            Target.Print(writer);
            writer.Write(" as ");
            writer.Write(Type.GetFriendlyName());
        }
    }
}