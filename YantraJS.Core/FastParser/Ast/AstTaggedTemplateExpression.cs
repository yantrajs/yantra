namespace YantraJS.Core.FastParser
{
    public class AstTaggedTemplateExpression : AstExpression
    {
        public readonly AstExpression Tag;

        public readonly ArraySpan<AstExpression> Arguments;

        public AstTaggedTemplateExpression(AstExpression tag, in ArraySpan<AstExpression> arguments)
            : base(arguments.FirstOrDefault().Start, FastNodeType.TaggedTemplateExpression, arguments.LastOrDefault().End)
        {
            this.Arguments = arguments;
            this.Tag = tag;
        }
    }
}