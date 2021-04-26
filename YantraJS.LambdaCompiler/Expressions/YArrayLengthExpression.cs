namespace YantraJS.Expressions
{
    public class YArrayLengthExpression: YExpression
    {
        public readonly YExpression Target;

        public YArrayLengthExpression(YExpression target)
            : base(YExpressionType.ArrayLength, typeof(int))
        {
            this.Target = target;
        }
    }
}