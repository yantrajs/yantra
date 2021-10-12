namespace YantraJS.Core.FastParser
{
    public class AstTemplateExpression : AstExpression
    {
        public readonly IFastEnumerable<AstExpression> Parts;

        public AstTemplateExpression(FastToken token, FastToken previousToken, IFastEnumerable<AstExpression> astExpressions)
            : base(token, FastNodeType.TemplateExpression, previousToken)
        {
            this.Parts = astExpressions;
        }
    }
}