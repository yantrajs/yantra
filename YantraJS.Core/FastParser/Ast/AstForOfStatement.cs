namespace YantraJS.Core.FastParser
{
    public class AstForOfStatement : AstStatement
    {
        public readonly AstNode Init;
        public readonly AstExpression Target;
        public readonly AstStatement Body;

        public AstForOfStatement(FastToken token, FastToken previousToken, AstNode beginNode, AstExpression target, AstStatement statement)
            : base(token, FastNodeType.ForOfStatement, previousToken)
        {
            this.Init = beginNode;
            this.Target = target;
            this.Body = statement;
        }

        public override string ToString()
        {
            return $"for ({Init} of {Target}) {{ {Body} }}";
        }

    }
}