using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{
    public class YBoxExpression : YExpression
    {
        public readonly YExpression Target;

        public YBoxExpression(YExpression target)
            : base(YExpressionType.Box, typeof(object))
        {
            this.Target = target;
        }

        public override void Print(IndentedTextWriter writer)
        {
            Target.Print(writer);
            writer.Write(" as object");
        }
    }
}
