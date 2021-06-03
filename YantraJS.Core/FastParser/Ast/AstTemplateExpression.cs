namespace YantraJS.Core.FastParser
{
    public class AstTemplateExpression : AstExpression
    {
        public readonly ArraySpan<AstExpression> Parts;

        public AstTemplateExpression(FastToken token, FastToken previousToken, in ArraySpan<AstExpression> astExpressions)
            : base(token, FastNodeType.TemplateExpression, previousToken)
        {
            this.Parts = astExpressions;
        }
    }
}