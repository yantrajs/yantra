namespace YantraJS.Expressions
{
    public class YBinaryExpression : YExpression
    {
        public readonly YExpression Left;
        public readonly YOperator Operator;
        public readonly YExpression Right;

        public YBinaryExpression(YExpression left, YOperator @operator, YExpression right)
            : base(YExpressionType.Binary, left.Type)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }
    }
}