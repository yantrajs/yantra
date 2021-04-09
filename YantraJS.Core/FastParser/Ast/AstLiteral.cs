namespace YantraJS.Core.FastParser
{
    public class AstLiteral : AstExpression
    {
        public readonly TokenTypes TokenType;

        public readonly double NumericValue;

        public string StringValue => Start.CookedText;

        public AstLiteral(TokenTypes tokenType, FastToken token): base(token, FastNodeType.Literal, token)
        {
            this.TokenType = tokenType;
            NumericValue = 0;
        }

        public override string ToString()
        {
            return TokenType.ToString();
        }
    }

    public class AstSuper: AstExpression
    {
        public readonly TokenTypes TokenType;

        public AstSuper(FastToken token) : base(token, FastNodeType.Super, token)
        {
        }

        public override string ToString()
        {
            return "super";
        }
    }
}