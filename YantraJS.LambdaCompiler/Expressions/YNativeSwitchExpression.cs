#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YantraJS.Expressions
{
    public class YNativeSwitchExpression : YExpression
    {
        public readonly new YLabelTarget Break;
        public readonly YExpression Target;
        public readonly MethodInfo? CompareMethod;
        public readonly YExpression? Default;
        public readonly YSwitchCaseExpression[] Cases;

        public YNativeSwitchExpression(
            YLabelTarget @break,
            YExpression target,
            MethodInfo? method,
            YExpression? defaultBody,
            YSwitchCaseExpression[] cases) : base(YExpressionType.NativeSwitch, cases.Last().Body.Type)
        {
            this.Break = @break;
            this.Target = target;
            this.CompareMethod = method;
            this.Default = defaultBody;
            this.Cases = cases;
        }

        public override void Print(IndentedTextWriter writer) {
            writer.Write("switch(");
            Target.Print(writer);
            writer.WriteLine(") {");
            writer.Indent++;

            foreach (var @case in Cases)
            {
                foreach (var tv in @case.TestValues)
                {
                    writer.Write("case ");
                    tv.Print(writer);
                }
                writer.Indent++;

                @case.Body.Print(writer);
                writer.Indent--;
            }

            if (Default != null)
            {
                writer.WriteLine("default:");
                writer.Indent++;
                Default.Print(writer);
                writer.Indent--;
            }

            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}
