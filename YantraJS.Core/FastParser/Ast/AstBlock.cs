namespace YantraJS.Core.FastParser
{
    public class AstBlock : AstStatement
    {

        public readonly ArraySpan<AstStatement> Statements;

        protected AstBlock(FastToken start, FastNodeType type, FastToken end, ArraySpan<AstStatement> statements) : base(start, type, end)
        {
            this.Statements = statements;
        }


        public AstBlock(FastToken start, FastToken end, ArraySpan<AstStatement> list) : base(start, FastNodeType.Block, end)
        {
            this.Statements = list;
        }

    }

}
