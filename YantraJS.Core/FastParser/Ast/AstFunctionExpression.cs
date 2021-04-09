namespace YantraJS.Core.FastParser
{
    public class AstFunctionExpression : AstExpression
    {
        public bool Async;
        public readonly bool Generator;
        public readonly AstIdentifier Id;
        public readonly ArraySpan<VariableDeclarator> Params;
        public readonly AstStatement Body;
        public readonly bool IsArrowFunction;

        public AstFunctionExpression(
            FastToken token, 
            FastToken previousToken, 
            bool isArrow,
            bool isAsync, 
            bool generator, 
            AstIdentifier id, 
            in ArraySpan<VariableDeclarator> declarators, AstStatement body)
            : base(token, FastNodeType.FunctionExpression, previousToken)
        {
            this.IsArrowFunction = isArrow;
            this.Async = isAsync;
            this.Generator = generator;
            this.Id = id;
            this.Params = declarators;
            this.Body = body;
        }
    }
}
