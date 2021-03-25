namespace YantraJS.Core.FastParser
{
    public class AstLiteral : AstExpression
    {
        public readonly TokenTypes TokenType;

        public AstLiteral(TokenTypes tokenType, FastToken token): base(token, FastNodeType.Literal, token)
        {
            this.TokenType = tokenType;
        }
    }
}