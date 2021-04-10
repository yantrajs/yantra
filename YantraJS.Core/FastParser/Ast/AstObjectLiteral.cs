namespace YantraJS.Core.FastParser
{
    public class AstObjectLiteral : AstExpression
    {
        public readonly ArraySpan<ObjectProperty> Properties;

        public AstObjectLiteral(
            FastToken token, 
            FastToken previousToken, 
            in ArraySpan<ObjectProperty> objectProperties)
            : base (token, FastNodeType.ObjectLiteral, previousToken)
        {
            this.Properties = objectProperties;
        }
    }
}