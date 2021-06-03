namespace YantraJS.Core.FastParser
{
    public class AstBindingPattern : AstExpression
    {
        public AstBindingPattern(FastToken start, FastNodeType type, FastToken end) : base(start, type, end, true)
        {
        }
    }

}
