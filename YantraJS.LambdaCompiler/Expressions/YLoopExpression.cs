namespace YantraJS.Expressions
{
    public class YLoopExpression: YExpression
    {
        public readonly YExpression Body;
        public readonly YLabelTarget Break;
        public readonly YLabelTarget Continue;

        public YLoopExpression(YExpression body, YLabelTarget @break, YLabelTarget @continue)
            : base(YExpressionType.Loop, @break.LabelType)
        {
            this.Body = body;
            this.Break = @break;
            this.Continue = @continue;
        }
    }
}