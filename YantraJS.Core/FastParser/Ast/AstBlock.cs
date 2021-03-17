namespace YantraJS.Core.FastParser
{
    public class AstBlock : AstStatement
    {

        public readonly AstStatement[] Statements;

        protected AstBlock(FastToken start, FastNodeType type, FastToken end, AstStatement[] statements) : base(start, type, end)
        {
            this.Statements = statements;
        }


        public AstBlock(FastToken start, FastToken end, AstStatement[] list) : base(start, FastNodeType.Block, end)
        {
            this.Statements = list;
        }

    }

}
