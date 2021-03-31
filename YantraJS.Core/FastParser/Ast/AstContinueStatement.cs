#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstContinueStatement : AstStatement
    {
        public readonly AstIdentifier? Label;

        public AstContinueStatement(FastToken token, FastToken previousToken, AstIdentifier? label = null)
            : base(token, FastNodeType.ContinueStatement, previousToken)
        {
            this.Label = label;
        }

        public override string ToString()
        {
            if (Label == null)
                return "continue;";
            return $"continue {Label};";
        }
    }

}