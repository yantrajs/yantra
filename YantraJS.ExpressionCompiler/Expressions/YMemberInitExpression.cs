using System.CodeDom.Compiler;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public class YMemberInitExpression: YExpression
    {
        public readonly YNewExpression Target;
        public readonly IFastEnumerable<YBinding> Bindings;

        public YMemberInitExpression(YNewExpression exp, IFastEnumerable<YBinding> list)
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
            var en = Bindings.GetFastEnumerator();
            while(en.MoveNext(out var b))
            {
                writer.Write(b.Member.Name);
                writer.Write(" = ");
                // b.Value.Print(writer);
                writer.WriteLine(",");
            }
            writer.Indent--;
            writer.Write("}");
        }
    }
}