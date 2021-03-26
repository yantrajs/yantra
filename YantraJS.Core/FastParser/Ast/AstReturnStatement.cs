namespace YantraJS.Core.FastParser
{
    public class AstReturnStatement : AstStatement
    {
        public readonly AstExpression Target;

        public AstReturnStatement(FastToken token, FastToken previousToken, AstExpression target = null)
            : base(token, FastNodeType.ContinueStatement, previousToken)
        {
            this.Target = target;
        }
    }
}