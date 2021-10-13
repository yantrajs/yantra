using System.CodeDom.Compiler;

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

        public override void Print(IndentedTextWriter writer)
        {
            switch (Operator)
            {
                case YUnaryOperator.Not:
                    writer.Write("~(");
                    Target.Print(writer);
                    writer.Write(")");
                    break;
                case YUnaryOperator.Negative:
                    writer.Write("!(");
                    Target.Print(writer);
                    writer.Write(")");
                    break;
            }
        }
    }
}