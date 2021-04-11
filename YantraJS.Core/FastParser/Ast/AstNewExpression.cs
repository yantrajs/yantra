namespace YantraJS.Core.FastParser
{
    public class AstNewExpression : AstExpression
    {
        public readonly AstExpression Callee;
        public readonly ArraySpan<AstExpression> Arguments;

        public AstNewExpression(FastToken begin, 
            AstExpression node,
            in ArraySpan<AstExpression> arguments): base(begin, FastNodeType.NewExpression, node.End)
        {
            this.Callee = node;
            this.Arguments = arguments;
        }
    }
}