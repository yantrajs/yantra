namespace YantraJS.Core.FastParser
{
    public class AstBlock : AstStatement
    {

        public readonly ArraySpan<AstStatement> Statements;

        protected AstBlock(FastToken start, FastNodeType type, FastToken end, in ArraySpan<AstStatement> statements) : base(start, type, end)
        {
            this.Statements = statements;
        }


        public AstBlock(FastToken start, FastToken end, in ArraySpan<AstStatement> list) : base(start, FastNodeType.Block, end)
        {
            this.Statements = list;
        }

        public override string ToString()
        {
            return Statements.Join("\n\t");
        }

    }

}
