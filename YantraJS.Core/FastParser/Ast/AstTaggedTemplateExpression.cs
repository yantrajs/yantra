namespace YantraJS.Core.FastParser
{
    public class AstTaggedTemplateExpression : AstExpression
    {
        public readonly ArraySpan<AstExpression> Arguments;

        public AstTaggedTemplateExpression(in ArraySpan<AstExpression> arguments)
            : base(arguments.FirstOrDefault().Start, FastNodeType.TaggedTemplateExpression, arguments.LastOrDefault().End)
        {
            this.Arguments = arguments;
        }
    }
}