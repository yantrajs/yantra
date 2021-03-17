namespace YantraJS.Core.FastParser
{
    public class AstArrayPattern : AstBindingPattern
    {
        public readonly AstExpression[] Elements;

        public AstArrayPattern(FastToken start, FastToken end, AstExpression[] elements) : base(start, FastNodeType.ObjectPattern, end)
        {
            this.Elements = elements;
        }
    }

}
