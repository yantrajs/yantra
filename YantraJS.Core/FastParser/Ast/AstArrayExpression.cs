#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstArrayExpression : AstExpression
    {
        public readonly ArraySpan<AstExpression> Elements;

        public AstArrayExpression(FastToken start, FastToken end, in ArraySpan<AstExpression> nodes)
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