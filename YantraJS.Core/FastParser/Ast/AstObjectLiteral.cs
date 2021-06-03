namespace YantraJS.Core.FastParser
{
    public class AstObjectLiteral : AstExpression
    {
        public readonly ArraySpan<AstNode> Properties;

        public AstObjectLiteral(
            FastToken token, 
            FastToken previousToken, 
            in ArraySpan<AstNode> objectProperties)
            : base (token, FastNodeType.ObjectLiteral, previousToken)
        {
            this.Properties = objectProperties;
        }
    }
}