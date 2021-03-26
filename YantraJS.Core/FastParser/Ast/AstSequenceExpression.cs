namespace YantraJS.Core.FastParser
{
    public class AstSequenceExpression : AstExpression
    {
        public readonly ArraySpan<AstExpression> Expressions;

        public AstSequenceExpression(FastToken start, FastToken end, ArraySpan<AstExpression> expressions) : base(start, FastNodeType.SequenceExpression, end)
        {
            this.Expressions = expressions;
        }
    }

}
