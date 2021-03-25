namespace YantraJS.Core.FastParser
{
    public class AstArrayExpression : AstExpression
    {
        public readonly AstExpression[] Elements;

        public AstArrayExpression(FastToken start, FastToken end, AstExpression[] nodes)
            : base(start, FastNodeType.ArrayExpression, end)
        {
            this.Elements = nodes;
        }
    }
}