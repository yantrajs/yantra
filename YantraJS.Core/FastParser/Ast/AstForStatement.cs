namespace YantraJS.Core.FastParser
{
    public class AstForStatement : AstStatement
    {
        public readonly AstNode Init;
        public readonly AstExpression Test;
        public readonly AstExpression Update;
        public readonly AstStatement Body;

        public AstForStatement(FastToken token, FastToken previousToken, AstNode beginNode, AstExpression test, AstExpression preTest, AstStatement statement)
            :base(token, FastNodeType.ForStatement, previousToken)
        {
            this.Init = beginNode;
            this.Test = test;
            this.Update = preTest;
            this.Body = statement;
        }

        public override string ToString()
        {
            return $"for ({Init};{Update};{Test}) {{ {Body} }}";
        }
    }
}