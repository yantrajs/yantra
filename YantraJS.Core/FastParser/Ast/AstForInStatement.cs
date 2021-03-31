namespace YantraJS.Core.FastParser
{
    public class AstForInStatement : AstStatement
    {
        public readonly AstNode Init;
        public readonly AstExpression Target;
        public readonly AstStatement Body;

        public AstForInStatement(FastToken token, FastToken previousToken, AstNode beginNode, AstExpression target, AstStatement statement)
            : base(token, FastNodeType.ForInStatement, previousToken)
        {
            this.Init = beginNode;
            this.Target = target;
            this.Body = statement;
        }

        public override string ToString()
        {
            return $"for ({Init} in {Target}) {{ {Body} }}";
        }

    }
}