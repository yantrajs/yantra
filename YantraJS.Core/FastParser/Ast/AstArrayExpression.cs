#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstArrayExpression : AstExpression
    {
        public readonly IFastEnumerable<AstExpression> Elements;

        public AstArrayExpression(FastToken start, FastToken end, IFastEnumerable<AstExpression> nodes)
            : base(start, FastNodeType.ArrayExpression, end)
        {
            this.Elements = nodes;
        }

        public override string ToString()
        {
            return $"[{Elements.Join()}]";
        }
    }
}