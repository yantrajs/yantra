namespace YantraJS.Core.FastParser
{
    public class AstCallExpression : AstExpression
    {
        public readonly AstExpression Callee;
        public readonly ArraySpan<AstExpression> Arguments;
        public readonly bool Coalesce;

        public AstCallExpression(
            AstExpression previous, 
            in ArraySpan<AstExpression> plist,
            bool coalesce = false)
            : base(previous.Start, FastNodeType.CallExpression, plist.Length > 0 ? plist[plist.Length-1].End : previous.End)
        {
            this.Callee = previous;
            this.Arguments = plist;
            this.Coalesce = coalesce;
        }

        public override string ToString()
        {
            return $"{Callee}({Arguments.Join()})";
        }
    }
}