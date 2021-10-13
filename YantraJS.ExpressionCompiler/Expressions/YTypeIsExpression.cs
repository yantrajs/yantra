using System;
using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YTypeIsExpression: YExpression
    {
        public readonly YExpression Target;
        public readonly Type TypeOperand;

        public YTypeIsExpression(YExpression target, Type type)
            : base(YExpressionType.TypeIs, typeof(bool))
        {
            this.Target = target;
            this.TypeOperand = type;
        }

        public override void Print(IndentedTextWriter writer)
        {
            Target.Print(writer);
            writer.Write(" is ");
            writer.Write(TypeOperand.GetFriendlyName());
        }
    }
}