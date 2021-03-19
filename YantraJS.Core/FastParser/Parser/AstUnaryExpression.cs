namespace YantraJS.Core.FastParser
{
    internal class AstUnaryExpression : AstExpression
    {
        public readonly AstExpression Argument;
        public readonly TokenTypes Operator;
        public readonly bool Prefix;

        public AstUnaryExpression(FastToken token, AstExpression argument, TokenTypes tokenType, bool prefix = true)
            : base(token, FastNodeType.UnaryExpression, argument.End)
        {
            this.Argument = argument;
            this.Operator = tokenType;
            this.Prefix = prefix;
        }
    }
}