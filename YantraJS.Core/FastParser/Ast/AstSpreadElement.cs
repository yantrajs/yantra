namespace YantraJS.Core.FastParser
{
    public class AstSpreadElement : AstExpression
    {
        public readonly AstExpression Element;

        public AstSpreadElement(FastToken start, FastToken end, AstExpression element) : base(start, FastNodeType.SpreadElement, end)
        {
            this.Element = element;
        }
    }

}
