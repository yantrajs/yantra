using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public class YJumpSwitchExpression : YExpression
    {
        public readonly YExpression Target;
        public readonly IFastEnumerable<YLabelTarget> Cases;

        public YJumpSwitchExpression(YExpression target, IFastEnumerable<YLabelTarget> cases) : base(YExpressionType.JumpSwitch, typeof(void))
        {
            this.Target = target;
            this.Cases = cases;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("switch (");
            Target.Print(writer);
            writer.WriteLine(") {");
            writer.Indent++;
            int i = 0;
            var en = Cases.GetFastEnumerator();
            while(en.MoveNext(out var label))
            {
                writer.Write("case ");
                writer.Write(i++);
                writer.Write(": goto ");
                writer.Write(label.Name);
                writer.WriteLine(";");
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}
