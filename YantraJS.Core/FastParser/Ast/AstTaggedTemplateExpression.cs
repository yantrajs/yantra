namespace YantraJS.Core.FastParser
{
    public class AstTaggedTemplateExpression : AstExpression
    {
        public readonly AstExpression Tag;
        public readonly AstExpression Template;

        public AstTaggedTemplateExpression(AstExpression tag, AstExpression template)
            : base(tag.Start, FastNodeType.TaggedTemplateExpression, template.End)
        {
            this.Tag = tag;
            this.Template = template;
        }
    }
}