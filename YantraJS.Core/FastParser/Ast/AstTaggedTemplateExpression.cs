namespace YantraJS.Core.FastParser
{
    public class AstTaggedTemplateExpression : AstExpression
    {
        public readonly AstExpression Tag;
        public readonly AstTemplateExpression Template;

        public AstTaggedTemplateExpression(AstExpression tag, AstTemplateExpression template)
            : base(tag.Start, FastNodeType.TaggedTemplateExpression, template.End)
        {
            this.Tag = tag;
            this.Template = template;
        }
    }
}