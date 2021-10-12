namespace YantraJS.Core.FastParser
{
    public class AstNewExpression : AstExpression
    {
        public readonly AstExpression Callee;
        public readonly IFastEnumerable<AstExpression> Arguments;

        public AstNewExpression(FastToken begin, 
            AstExpression node,
            IFastEnumerable<AstExpression> arguments): base(begin, FastNodeType.NewExpression, node.End)
        {
            this.Callee = node;
            this.Arguments = arguments;
        }

        public override string ToString()
        {
            return $"new {Callee}({Arguments.Join()})";
        }
    }
}