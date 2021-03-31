namespace YantraJS.Core.FastParser
{
    public class AstObjectPattern : AstBindingPattern
    {
        public readonly ArraySpan<ObjectProperty> Properties;

        public AstObjectPattern(
            FastToken start, 
            FastToken end, 
            in ArraySpan<ObjectProperty> properties) : base(start, FastNodeType.ObjectPattern, end)
        {
            this.Properties = properties;
        }
    }

}
