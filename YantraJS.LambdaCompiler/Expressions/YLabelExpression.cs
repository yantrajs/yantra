namespace YantraJS.Expressions
{
    public class YLabelExpression: YExpression
    {
        public readonly YLabelTarget Target;
        public readonly YExpression Default;

        public YLabelExpression(YLabelTarget target, YExpression defaultValue)
            : base(YExpressionType.Label, target.LabelType)
        {
            this.Target = target;
            this.Default = defaultValue;
        }
    }

    public class YGoToExpression : YExpression
    {
        public readonly YLabelTarget Target;

        public readonly YExpression Default;

        public YGoToExpression(YLabelTarget target, YExpression defaultValue)
            : base(YExpressionType.GoTo, target.LabelType)
        {
            this.Target = target;
            this.Default = defaultValue;
        }
    }
    public class YReturnExpression : YExpression
    {
        public readonly YLabelTarget Target;
        public readonly YExpression Default;

        public YReturnExpression(YLabelTarget target, YExpression defaultValue)
            : base(YExpressionType.Return, target.LabelType)
        {
            this.Target = target;
            this.Default = defaultValue;
        }
    }

}