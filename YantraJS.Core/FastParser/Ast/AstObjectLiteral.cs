namespace YantraJS.Core.FastParser
{
    public class AstObjectLiteral : AstExpression
    {
        public readonly IFastEnumerable<AstNode> Properties;

        public AstObjectLiteral(
            FastToken token, 
            FastToken previousToken,
            IFastEnumerable<AstNode> objectProperties)
            : base (token, FastNodeType.ObjectLiteral, previousToken)
        {
            this.Properties = objectProperties;
        }
    }
}