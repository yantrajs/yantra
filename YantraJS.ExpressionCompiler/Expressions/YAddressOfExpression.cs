using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{
    public class YAddressOfExpression : YExpression
    {
        public readonly YExpression Target;

        public YAddressOfExpression(YExpression target)
            : base(YExpressionType.AddressOf, target.Type.IsByRef ? target.Type : target.Type.MakeByRefType())
        {
            this.Target = target;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("ref ");
            Target.Print(writer);
        }
    }
}
