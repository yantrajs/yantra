namespace YantraJS.Core.FastParser
{
    public class AstExpressionStatement : AstStatement
    {
        public readonly AstExpression Expression;

        public AstExpressionStatement(FastToken start, FastToken end, AstExpression expression) 
            : base(start, FastNodeType.ExpressionStatement, end)
        {
            this.Expression = expression;
        }
    }

}
