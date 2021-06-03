namespace YantraJS.Core.FastParser
{
    public class AstLiteral : AstExpression
    {
        public readonly TokenTypes TokenType;

        public double NumericValue => Start.Number;

        public string StringValue => Start.CookedText ?? Start.Span.Value;

        public (string Pattern, string Flags) Regex => (this.Start.CookedText, this.Start.Flags);

        public AstLiteral(TokenTypes tokenType, FastToken token): base(token, FastNodeType.Literal, token)
        {
            this.TokenType = tokenType;
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