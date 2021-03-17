namespace YantraJS.Core.FastParser
{
    public class AstSequenceExpression : AstExpression
    {
        public readonly AstExpression[] Expressions;

        public AstSequenceExpression(FastToken start, FastToken end, AstExpression[] expressions) : base(start, FastNodeType.SequenceExpression, end)
        {
            this.Expressions = expressions;
        }
    }

}
