namespace YantraJS.Core.FastParser
{
    public class AstSwitchStatement : AstStatement
    {
        public readonly AstExpression Target;
        public readonly ArraySpan<AstCase> Cases;

        public AstSwitchStatement(FastToken start, FastToken end, AstExpression target, in ArraySpan<AstCase> astCases)
            : base(start, FastNodeType.SwitchStatement, end)
        {
            this.Target = target;
            this.Cases = astCases;
        }
    }
}