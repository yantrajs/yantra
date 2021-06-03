namespace YantraJS.Core.FastParser
{
    public class AstWhileStatement : AstStatement
    {
        public readonly AstExpression Test;
        public readonly AstStatement Body;

        public AstWhileStatement(FastToken start, FastToken end, AstExpression test, AstStatement statement)
            : base(start, FastNodeType.WhileStatement, end)
        {
            this.Test = test;
            this.Body = statement;
        }
    }

}
