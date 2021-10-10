namespace YantraJS.Core.FastParser
{
    public class AstTaggedTemplateExpression : AstExpression
    {
        public readonly AstExpression Tag;

        public readonly IFastEnumerable<AstExpression> Arguments;

        public AstTaggedTemplateExpression(AstExpression tag, IFastEnumerable<AstExpression> arguments)
            : base(arguments.FirstOrDefault().Start, FastNodeType.TaggedTemplateExpression, arguments.LastOrDefault().End)
        {
            this.Arguments = arguments;
            this.Tag = tag;
        }
    }
}