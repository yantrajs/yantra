namespace YantraJS.Core.FastParser
{
    public class AstDoWhileStatement : AstStatement
    {
        public readonly AstExpression Test;
        public readonly AstStatement Body;

        public AstDoWhileStatement(
            FastToken start, 
            FastToken end, 
            AstExpression test, 
            AstStatement statement) : base(start, FastNodeType.DoWhileStatement, end)
        {
            this.Test = test;
            this.Body = statement;
        }

        public override string ToString()
        {
            return @$"do {{
    {Body} 
}} while ({Test})";
        }
    }

}
