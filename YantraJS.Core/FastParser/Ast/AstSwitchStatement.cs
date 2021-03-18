namespace YantraJS.Core.FastParser
{
    internal class AstSwitchStatement : AstStatement
    {
        public readonly AstExpression Target;
        public readonly AstCase[] Cases;

        public AstSwitchStatement(FastToken start, FastToken end, AstExpression target, AstCase[] astCases)
            : base(start, FastNodeType.SwitchStatement, end)
        {
            this.Target = target;
            this.Cases = astCases;
        }
    }
}