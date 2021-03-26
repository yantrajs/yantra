namespace YantraJS.Core.FastParser
{
    public class AstYieldExpression : AstExpression
    {
        public readonly AstExpression Argument;
        public readonly bool Delegate;

        public AstYieldExpression(FastToken token, FastToken previousToken, AstExpression target, bool @delegate = false)
            : base(token, FastNodeType.YieldExpression, previousToken)
        {
            this.Argument = target;
            this.Delegate = @delegate;
        }
    }
}