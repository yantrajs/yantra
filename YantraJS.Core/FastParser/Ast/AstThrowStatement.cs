namespace YantraJS.Core.FastParser
{
    public class AstThrowStatement : AstStatement
    {
        public readonly AstExpression Argument;

        public AstThrowStatement(FastToken token, FastToken previousToken, AstExpression target)
            : base(token, FastNodeType.ThrowStatement, previousToken)
        {
            this.Argument = target;
        }
    }
}