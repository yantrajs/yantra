#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstBlock : AstStatement
    {

        /// <summary>
        /// 
        /// </summary>
        public IFastEnumerable<StringSpan>? HoistingScope;


        public readonly IFastEnumerable<AstStatement> Statements;

        protected AstBlock(FastToken start, FastNodeType type, FastToken end, IFastEnumerable<AstStatement> statements) : base(start, type, end)
        {
            this.Statements = statements;
        }


        public AstBlock(FastToken start, FastToken end, IFastEnumerable<AstStatement> list) : base(start, FastNodeType.Block, end)
        {
            this.Statements = list;
        }

        public override string ToString()
        {
            return Statements.Join("\n\t");
        }

    }

}
