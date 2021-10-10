namespace YantraJS.Core.FastParser
{
    public class AstSwitchStatement : AstStatement
    {
        public readonly AstExpression Target;
        public readonly IFastEnumerable<AstCase> Cases;

        public AstSwitchStatement(FastToken start, FastToken end, AstExpression target, IFastEnumerable<AstCase> astCases)
            : base(start, FastNodeType.SwitchStatement, end)
        {
            this.Target = target;
            this.Cases = astCases;
        }
    }
}