namespace YantraJS.Core.FastParser
{
    public class AstArrayPattern : AstBindingPattern
    {
        public readonly ArraySpan<AstExpression> Elements;

        public AstArrayPattern(FastToken start, FastToken end, in ArraySpan<AstExpression> elements) : base(start, FastNodeType.ObjectPattern, end)
        {
            this.Elements = elements;
        }
    }

}
