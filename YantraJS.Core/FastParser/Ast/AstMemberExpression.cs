#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstMemberExpression : AstExpression
    {
        public readonly AstExpression Target;
        public readonly AstExpression Member;
        public readonly bool Computed;

        public AstMemberExpression(AstExpression target, AstExpression node, bool computed = false):
            base(target.End, FastNodeType.MemberExpression, node.End)
        {
            this.Target = target;
            this.Member = node;
            this.Computed = computed;
        }

        public override string ToString()
        {
            if(Computed)
                return $"{Target}[{Member}]";
            return $"{Target}.{Member}";
        }
    }
}