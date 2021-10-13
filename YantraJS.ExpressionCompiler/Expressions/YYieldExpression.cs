using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YYieldExpression: YExpression
    {
        public readonly YExpression Argument;

        public YYieldExpression(YExpression arg): 
            base(YExpressionType.Yield, arg.Type)
        {
            this.Argument = arg;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("yield ");
            Argument.Print(writer);
        }
    }
}