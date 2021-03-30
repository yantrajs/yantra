namespace YantraJS.Core.FastParser
{
    public class AstCallExpression : AstExpression
    {
        public readonly AstExpression Target;
        public readonly ArraySpan<AstExpression> Arguments;

        public AstCallExpression(AstExpression previous, ArraySpan<AstExpression> plist)
            : base(previous.Start, FastNodeType.CallExpression, plist.Length > 0 ? plist[plist.Length-1].End : previous.End)
        {
            this.Target = previous;
            this.Arguments = plist;
        }
    }
}