namespace YantraJS.Core.FastParser
{
    internal class AstBinaryExpression : AstExpression
    {
        public readonly AstExpression Left;
        public readonly TokenTypes Operator;
        public readonly AstExpression Right;

        public AstBinaryExpression(AstExpression node, TokenTypes type, AstExpression right)
            : base (node.Start, FastNodeType.BinaryExpression, right.End)
        {
            this.Left = node;
            this.Operator = type;
            this.Right = right;
        }
    }
}