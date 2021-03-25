namespace YantraJS.Core.FastParser
{
    internal class AstCallExpression : AstExpression
    {
        public readonly AstExpression Target;
        public readonly AstExpression[] Arguments;

        public AstCallExpression(AstExpression previous, AstExpression[] plist)
            : base(previous.Start, FastNodeType.CallExpression, plist.Length > 0 ? plist[plist.Length-1].End : previous.End)
        {
            this.Target = previous;
            this.Arguments = plist;
        }
    }
}