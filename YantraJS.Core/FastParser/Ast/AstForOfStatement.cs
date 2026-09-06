namespace YantraJS.Core.FastParser
{
    public class AstForOfStatement : AstStatement
    {
        public readonly AstNode Init;
        public readonly AstExpression Target;
        public readonly AstStatement Body;

        public readonly bool IsAsync;

        public AstForOfStatement(
            FastToken token,
            FastToken previousToken,
            AstNode beginNode,
            AstExpression target,
            AstStatement statement,
            bool isAsync = false)
            : base(token, FastNodeType.ForOfStatement, previousToken)
        {
            this.Init = beginNode;
            this.Target = target;
            this.Body = statement;
            this.IsAsync = isAsync;
        }

        public override string ToString()
        {
            return $"for ({Init} of {Target}) {{ {Body} }}";
        }

    }
}