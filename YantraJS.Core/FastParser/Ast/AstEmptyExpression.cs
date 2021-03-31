namespace YantraJS.Core.FastParser
{
    public class AstEmptyExpression : AstExpression
    {
        public AstEmptyExpression(FastToken start, bool isBinding = false)
            : base(start, FastNodeType.EmptyExpression, start, isBinding)
        {
        }

        public override string ToString()
        {
            return "<<Empty>>";
        }
    }

}
