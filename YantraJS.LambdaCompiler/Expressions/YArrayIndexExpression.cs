namespace YantraJS.Expressions
{
    public class YArrayIndexExpression: YExpression
    {
        public readonly YExpression Target;
        public readonly YExpression Index;

        public YArrayIndexExpression(YExpression target, YExpression index)
            : base(YExpressionType.ArrayIndex, target.Type.GetElementType())
        {
            this.Target = target;
            this.Index = index;
        }
    }
}