namespace YantraJS.Core.FastParser
{
    public class AstUnaryExpression : AstExpression
    {
        public readonly AstExpression Argument;
        public readonly UnaryOperator Operator;
        public readonly bool Prefix;

        public AstUnaryExpression(FastToken token, AstExpression argument, UnaryOperator tokenType, bool prefix = true)
            : base(token, FastNodeType.UnaryExpression, argument.End)
        {
            this.Argument = argument;
            this.Operator = tokenType;
            this.Prefix = prefix;
        }
    }
}