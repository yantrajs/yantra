namespace YantraJS.Core.FastParser
{
    internal class AstNewExpression : AstExpression
    {
        public readonly AstExpression Argument;

        public AstNewExpression(FastToken begin, AstExpression node): base(begin, FastNodeType.NewExpression, node.End)
        {
            this.Argument = node;
        }
    }
}