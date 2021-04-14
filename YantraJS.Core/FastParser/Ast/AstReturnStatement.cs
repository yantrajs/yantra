#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstReturnStatement : AstStatement
    {
        public readonly AstExpression? Argument;

        public AstReturnStatement(FastToken token, FastToken previousToken, AstExpression? target = null)
            : base(token, FastNodeType.ReturnStatement, previousToken)
        {
            this.Argument = target;
        }

        public override string ToString()
        {
            var hasSemiColonAtEnd = End.Type == TokenTypes.SemiColon ? ":" : "";
            if (Argument != null)
            {
                return $"return {Argument}{hasSemiColonAtEnd}";
            }
            return $"return {hasSemiColonAtEnd}";
        }
    }
}