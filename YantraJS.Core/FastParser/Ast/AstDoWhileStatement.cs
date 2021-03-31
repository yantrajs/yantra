namespace YantraJS.Core.FastParser
{
    public class AstDoWhileStatement : AstStatement
    {
        public readonly AstExpression Test;
        public readonly AstStatement Statement;

        public AstDoWhileStatement(
            FastToken start, 
            FastToken end, 
            AstExpression test, 
            AstStatement statement) : base(start, FastNodeType.DoWhileStatement, end)
        {
            this.Test = test;
            this.Statement = statement;
        }

        public override string ToString()
        {
            return @$"do {{
    {Statement} 
}} while ({Test})";
        }
    }

}
