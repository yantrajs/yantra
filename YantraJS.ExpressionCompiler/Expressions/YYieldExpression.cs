using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YYieldExpression: YExpression
    {
        public readonly YExpression Argument;
        public readonly bool DelegateYield;

        public YYieldExpression(YExpression arg, bool @delegate): 
            base(YExpressionType.Yield, arg.Type)
        {
            this.Argument = arg;
            this.DelegateYield = @delegate;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("yield ");
            if (DelegateYield)
            {
                writer.Write("*");
            }
            Argument.Print(writer);
        }
    }
}