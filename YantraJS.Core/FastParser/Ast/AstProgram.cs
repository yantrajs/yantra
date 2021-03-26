namespace YantraJS.Core.FastParser
{
    public class AstProgram : AstBlock
    {
        public AstProgram(FastToken token, FastToken end, ArraySpan<AstStatement> statements)
            : base(token, FastNodeType.Program, end, statements )
        {
        }
    }

}
