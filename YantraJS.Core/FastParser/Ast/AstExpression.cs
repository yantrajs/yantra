namespace YantraJS.Core.FastParser
{
    public class AstExpression : AstNode
    {
        public AstExpression(FastToken start, FastNodeType type, FastToken end, bool isBinding = false)
            : base(start, type, end, false, isBinding)
        {
        }

    }


    internal static class AstExpressionExtensions
    {

        public static AstExpression Computed(this AstExpression left, AstExpression right)
            => new AstMemberExpression(left, right, true);

        public static AstExpression Member(
            this AstExpression left, 
            AstExpression right, 
            bool computed = false,
            bool coalesce = false)
            => left == null ? right : new AstMemberExpression(left, right, computed, coalesce);

    }

}
