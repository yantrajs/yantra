namespace YantraJS.Core.FastParser
{
    public class AstObjectPattern : AstBindingPattern
    {
        public readonly ObjectProperty[] Properties;

        public AstObjectPattern(FastToken start, FastToken end, ObjectProperty[] properties) : base(start, FastNodeType.ObjectPattern, end)
        {
            this.Properties = properties;
        }
    }

}
