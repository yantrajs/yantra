#nullable enable
using System;
using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YLabelExpression: YExpression
    {
        public readonly YLabelTarget Target;
        public readonly YExpression? Default;

        public YLabelExpression(YLabelTarget target, YExpression? defaultValue)
            : base(YExpressionType.Label, target.LabelType)
        {
            this.Target = target;
            this.Default = defaultValue;
        }

        public override void Print(IndentedTextWriter writer)
        {
            if(Default != null)
            {
                writer.Write($"{Target.Name}: (");
                Default.Print(writer);
                writer.Write(")");
                return;
            }
            writer.WriteLine($"{Target.Name}:");
        }
    }

    public class YGoToExpression : YExpression
    {
        public readonly YLabelTarget Target;

        public readonly YExpression? Default;

        public YGoToExpression(YLabelTarget target, YExpression? defaultValue)
            : base(YExpressionType.GoTo, target.LabelType)
        {
            this.Target = target;
            this.Default = defaultValue;
        }

        public override void Print(IndentedTextWriter writer)
        {
            if(Default!=null){
                writer.Write($"Goto {Target.Name} with (");
                Default.Print(writer);
                writer.Write(")");
                return;
            }

            writer.Write($"Goto {Target.Name}");
        }
    }
    public class YReturnExpression : YExpression
    {
        public readonly YLabelTarget Target;
        public readonly YExpression? Default;

        public YReturnExpression(YLabelTarget target, YExpression? defaultValue)
            : base(YExpressionType.Return, defaultValue?.Type ?? typeof(void))
        {
            this.Target = target;
            this.Default = defaultValue;
        }

        public override void Print(IndentedTextWriter writer)
        {
            if (Default != null)
            {
                writer.Write("RETURN (");
                Default.Print(writer);
                writer.Write($") at {Target.Name}");
                return;
            }

            writer.Write($"RETURN {Target.Name}");
        }

        public YExpression Update(YLabelTarget target, YExpression x)
        {
            return new YReturnExpression(target, x);
        }
    }

}