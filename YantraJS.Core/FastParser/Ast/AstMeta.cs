namespace YantraJS.Core.FastParser
{
    public class AstMeta : AstExpression
    {
        public readonly AstIdentifier Identifier;
        public readonly AstIdentifier Property;

        public AstMeta(AstIdentifier id, AstIdentifier property)
            : base(id.Start, FastNodeType.Meta, property.End)
        {
            this.Identifier = id;
            this.Property = property;
        }
    }
}