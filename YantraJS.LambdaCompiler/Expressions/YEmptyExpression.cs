using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YEmptyExpression: YExpression
    {
        public YEmptyExpression()
            : base( YExpressionType.Empty, typeof(void))
        {
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("<void>");
        }
    }
}