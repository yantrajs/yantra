namespace YantraJS.Core.FastParser
{
    public class AstArrayPattern : AstBindingPattern
    {
        public readonly ArraySpan<AstExpression> Elements;

        public AstArrayPattern(FastToken start, FastToken end, in ArraySpan<AstExpression> elements) 
            : base(start, FastNodeType.ArrayPattern, end)
        {
            this.Elements = elements;
        }

        public override string ToString()
        {
            return $"[{Elements.Join()}]";
        }
    }

}
