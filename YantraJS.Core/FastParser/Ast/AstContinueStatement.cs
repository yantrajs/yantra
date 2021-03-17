namespace YantraJS.Core.FastParser
{
    internal class AstContinueStatement : AstStatement
    {
        public readonly AstIdentifier Label;

        public AstContinueStatement(FastToken token, FastToken previousToken, AstIdentifier label = null)
            : base(token, FastNodeType.ContinueStatement, previousToken)
        {
            this.Label = label;
        }
    }

}