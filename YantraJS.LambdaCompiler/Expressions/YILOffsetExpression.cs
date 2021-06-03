using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{
    public class YILOffsetExpression : YExpression
    {

        public YILOffsetExpression():
            base (YExpressionType.ILOffset, typeof(int))
        {

        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.WriteLine("// IL Offset");
        }
    }
}
