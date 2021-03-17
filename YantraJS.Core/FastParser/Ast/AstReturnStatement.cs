namespace YantraJS.Core.FastParser
{
    internal class AstReturnStatement : AstStatement
    {
        public readonly AstExpression Target;

        public AstReturnStatement(FastToken token, FastToken previousToken, AstExpression target = null)
            : base(token, FastNodeType.ContinueStatement, previousToken)
        {
            this.Target = target;
        }
    }

    internal class AstYieldStatement : AstStatement
    {
        public readonly AstExpression Argument;
        public readonly bool Delegate;

        public AstYieldStatement(FastToken token, FastToken previousToken, AstExpression target, bool @delegate = false)
            : base(token, FastNodeType.ContinueStatement, previousToken)
        {
            this.Argument = target;
            this.Delegate = @delegate;
        }
    }
}