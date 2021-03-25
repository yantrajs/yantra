namespace YantraJS.Core.FastParser
{
    internal class AstMemberExpression : AstExpression
    {
        public readonly AstExpression Target;
        public readonly AstExpression Member;
        public readonly bool Computed;

        public AstMemberExpression(AstExpression target, AstExpression node, bool computed = false):
            base(target.End, FastNodeType.MemberExpression, node.End)
        {
            this.Member = node;
            this.Computed = computed;
        }
    }
}