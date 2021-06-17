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
            switch (tokenType)
            {
                case UnaryOperator.Increment:
                case UnaryOperator.Decrement:
                    switch (argument.Type)
                    {
                        case FastNodeType.Identifier:
                        case FastNodeType.MemberExpression:
                            break;
                        default:
                            throw new FastParseException(token, $"Invalid expression for update");
                    }
                    break;
            }
            this.Argument = argument;
            this.Operator = tokenType;
            this.Prefix = prefix;
        }

        public override string ToString()
        {
            if (Prefix)
            {
                return $"{Operator} {Argument}";
            }
            return $"{Argument} {Operator}";
        }
    }
}