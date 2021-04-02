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

        public static AstExpression Member(this AstExpression left, AstExpression right, bool computed = false)
            => left == null ? right : new AstMemberExpression(left, right, computed);


        public static AstExpression Combine(this AstExpression left, 
            TokenTypes type, 
            AstExpression right)
        {
            if (right == null)
                return left;
            switch(type)
            {
                case TokenTypes.SemiColon:
                case TokenTypes.EOF:
                case TokenTypes.BracketEnd:
                case TokenTypes.SquareBracketEnd:
                case TokenTypes.CurlyBracketEnd:
                case TokenTypes.LineTerminator:
                    return left;
            }
            if (type == TokenTypes.Dot)
                return new AstMemberExpression(left, right);
            return new AstBinaryExpression(left, type, right);
        }
    }

}
