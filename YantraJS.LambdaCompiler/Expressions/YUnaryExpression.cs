namespace YantraJS.Expressions
{
    public class YUnaryExpression: YExpression
    {
        public readonly YExpression Target;
        public readonly YUnaryOperator Operator;

        public YUnaryExpression(YExpression exp, YUnaryOperator @operator)
            : base(YExpressionType.Unary, exp.Type)
        {
            this.Target = exp;
            this.Operator = @operator;
        }
    }
}