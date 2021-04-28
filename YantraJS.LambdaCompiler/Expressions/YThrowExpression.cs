using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YThrowExpression: YExpression
    {
        public readonly YExpression Expression;

        public YThrowExpression(YExpression exp)
            : base(YExpressionType.Throw, exp.Type)
        {
            this.Expression = exp;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("throw ");
            Expression.Print(writer);
        }
    }
}