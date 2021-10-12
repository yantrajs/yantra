namespace YantraJS.Core.FastParser
{
    public class AstCallExpression : AstExpression
    {
        public readonly AstExpression Callee;
        public readonly IFastEnumerable<AstExpression> Arguments;
        public readonly bool Coalesce;

        public AstCallExpression(
            AstExpression previous,
            IFastEnumerable<AstExpression> plist,
            bool coalesce = false)
            : base(previous.Start, FastNodeType.CallExpression, plist.Count > 0 ? plist.Last().End : previous.End)
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