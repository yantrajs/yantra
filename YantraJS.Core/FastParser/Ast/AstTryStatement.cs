namespace YantraJS.Core.FastParser
{
    public class AstTryStatement : AstStatement
    {
        public readonly AstStatement Block;
        public readonly AstIdentifier Identifier;
        public readonly AstStatement Catch;
        public readonly AstStatement Finally;

        public AstTryStatement(FastToken token, FastToken previousToken, AstStatement body, AstIdentifier id, AstStatement @catch, AstStatement @finally)
            : base(token, FastNodeType.TryStatement, previousToken)
        {
            this.Block = body;
            this.Identifier = id;
            this.Catch = @catch;
            this.Finally = @finally;
        }
    }
}