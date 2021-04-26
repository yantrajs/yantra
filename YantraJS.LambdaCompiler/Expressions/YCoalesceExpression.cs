namespace YantraJS.Expressions
{
    public class YCoalesceExpression: YExpression
    {
        public readonly YExpression Left;
        public readonly YExpression Right;

        public YCoalesceExpression(YExpression left, YExpression right)
            : base(YExpressionType.Coalesce, left.Type)
        {
            this.Left = left;
            this.Right = right;
        }
    }
}