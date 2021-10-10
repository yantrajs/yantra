namespace YantraJS.Core.FastParser
{
    public class AstObjectPattern : AstBindingPattern
    {
        public readonly IFastEnumerable<ObjectProperty> Properties;

        public AstObjectPattern(
            FastToken start, 
            FastToken end,
            IFastEnumerable<ObjectProperty> properties) : base(start, FastNodeType.ObjectPattern, end)
        {
            this.Properties = properties;
        }
    }

}
