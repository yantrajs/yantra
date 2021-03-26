namespace YantraJS.Core.FastParser
{
    public class AstFunctionExpression : AstExpression
    {
        public bool Async;
        public readonly bool Generator;
        public readonly AstIdentifier Identifier;
        public readonly ArraySpan<VariableDeclarator> Parameters;
        public readonly AstStatement Body;

        public AstFunctionExpression(FastToken token, FastToken previousToken, bool isAsync, bool generator, AstIdentifier id, ArraySpan<VariableDeclarator> declarators, AstStatement body)
            : base(token, FastNodeType.FunctionExpression, previousToken)
        {
            this.Async = isAsync;
            this.Generator = generator;
            this.Identifier = id;
            this.Parameters = declarators;
            this.Body = body;
        }
    }
}
