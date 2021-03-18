namespace YantraJS.Core.FastParser
{
    internal class AstMemberExpression : AstExpression
    {
        public readonly AstExpression Member;
        public readonly bool Computed;

        public AstMemberExpression(FastToken token, FastToken previousToken, AstExpression node, bool v):
            base(token, FastNodeType.MemberExpression, previousToken)
        {
            this.Member = node;
            this.Computed = v;
        }
    }
}