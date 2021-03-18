namespace YantraJS.Core.FastParser
{
    internal class AstFunctionExpression : AstExpression
    {
        public readonly bool Async;
        public readonly bool Generator;
        public readonly AstIdentifier Identifier;
        public readonly VariableDeclarator[] Parameters;
        public readonly AstStatement Body;

        public AstFunctionExpression(FastToken token, FastToken previousToken, bool isAsync, bool generator, AstIdentifier id, VariableDeclarator[] declarators, AstStatement body)
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
