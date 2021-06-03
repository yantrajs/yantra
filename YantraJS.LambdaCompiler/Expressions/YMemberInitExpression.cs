using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YMemberInitExpression: YExpression
    {
        public readonly YNewExpression Target;
        public readonly YMemberAssignment[] Bindings;

        public YMemberInitExpression(YNewExpression exp, YMemberAssignment[] list)
            : base(YExpressionType.MemberInit, exp.Type)
        {
            this.Target = exp;
            this.Bindings = list;
        }

        public override void Print(IndentedTextWriter writer)
        {
            Target.Print(writer);
            writer.WriteLine("{");
            writer.Indent++;
            foreach(var b in Bindings)
            {
                writer.Write(b.Member.Name);
                writer.Write(" = ");
                b.Value.Print(writer);
                writer.WriteLine(",");
            }
            writer.Indent--;
            writer.Write("}");
        }
    }
}