namespace YantraJS.Core.FastParser
{
    public class AstBlock : AstStatement
    {

        public readonly SparseList<AstStatement> Statements;

        protected AstBlock(FastToken start, FastNodeType type, FastToken end, SparseList<AstStatement> statements) : base(start, type, end)
        {
            this.Statements = statements;
        }


        public AstBlock(FastToken start, FastToken end, SparseList<AstStatement> list) : base(start, FastNodeType.Block, end)
        {
            this.Statements = list;
        }

    }

}
