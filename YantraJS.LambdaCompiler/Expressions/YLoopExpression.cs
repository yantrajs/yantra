using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YLoopExpression: YExpression
    {
        public readonly YExpression Body;
        public readonly new YLabelTarget Break;
        public readonly new YLabelTarget Continue;

        public YLoopExpression(YExpression body, YLabelTarget @break, YLabelTarget @continue)
            : base(YExpressionType.Loop, @break.LabelType)
        {
            this.Body = body;
            this.Break = @break;
            this.Continue = @continue;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.WriteLine("while(true) {");
            writer.Indent++;
            Body.Print(writer);
            writer.Indent--;
            writer.WriteLine("}");
            writer.WriteLine($"{Break.Name}:");
        }
    }
}