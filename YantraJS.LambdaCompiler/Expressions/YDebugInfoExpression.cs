using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{
    public class YDebugInfoExpression: YExpression
    {
        public readonly Position Start;
        public readonly Position End;

        public YDebugInfoExpression(Position start, Position end)
            : base (YExpressionType.DebugInfo, typeof(void))
        {
            this.Start = start;
            this.End = end;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.WriteLine($"Sequence Point {Start} {End}");
        }
    }
}
