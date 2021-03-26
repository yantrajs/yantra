namespace YantraJS.Core.FastParser
{
    public class AstObjectLiteral : AstExpression
    {
        public readonly ArraySpan<ObjectProperty> Members;

        public AstObjectLiteral(FastToken token, FastToken previousToken, ArraySpan<ObjectProperty> objectProperties)
            : base (token, FastNodeType.ObjectLiteral, previousToken)
        {
            this.Members = objectProperties;
        }
    }
}