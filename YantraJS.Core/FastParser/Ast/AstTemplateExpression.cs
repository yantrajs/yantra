namespace YantraJS.Core.FastParser
{
    public class AstTemplateExpression : AstExpression
    {
        public readonly AstExpression[] Parts;

        public AstTemplateExpression(FastToken token, FastToken previousToken, AstExpression[] astExpressions)
            : base(token, FastNodeType.TemplateExpression, previousToken)
        {
            this.Parts = astExpressions;
        }
    }
}