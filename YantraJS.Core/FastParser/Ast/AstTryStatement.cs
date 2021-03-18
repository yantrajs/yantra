namespace YantraJS.Core.FastParser
{
    internal class AstTryStatement : AstStatement
    {
        public readonly AstStatement Body;
        public readonly AstIdentifier Identifier;
        public readonly AstStatement Catch;

        public AstTryStatement(FastToken token, FastToken previousToken, AstStatement body, AstIdentifier id, AstStatement @catch)
            : base(token, FastNodeType.TryStatement, previousToken)
        {
            this.Body = body;
            this.Identifier = id;
            this.Catch = @catch;
        }
    }
}