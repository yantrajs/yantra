using System.CodeDom.Compiler;

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

        public override void Print(IndentedTextWriter writer)
        {
            if(Default != null)
            {
                Default.Print(writer);
            }
            writer.WriteLine($"{Target.Name}:");
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

        public override void Print(IndentedTextWriter writer)
        {
            if(Default!=null){
                writer.Write("Push>>");
                Default.Print(writer);
                writer.Write("<<");
            }

            writer.Write($"Goto {Target.Name}");
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

        public override void Print(IndentedTextWriter writer)
        {
            if (Default != null)
            {
                writer.Write("Push>>");
                Default.Print(writer);
                writer.Write("<<");
            }

            writer.Write($"RETURN {Target.Name}");
        }
    }

}