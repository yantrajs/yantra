namespace YantraJS.Core.FastParser
{
    public class AstReturnStatement : AstStatement
    {
        public readonly AstExpression Argument;

        public AstReturnStatement(FastToken token, FastToken previousToken, AstExpression target = null)
            : base(token, FastNodeType.ReturnStatement, previousToken)
        {
            this.Argument = target;
        }
    }
}