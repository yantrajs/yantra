using System.CodeDom.Compiler;

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

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("(");
            Left.Print(writer);
            writer.Write(" ?? ");
            Right.Print(writer);
            writer.Write(")");
        }
    }
}