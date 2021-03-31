namespace YantraJS.Core.FastParser
{
    public class AstProgram : AstBlock
    {
        public AstProgram(FastToken token, FastToken end, in ArraySpan<AstStatement> statements)
            : base(token, FastNodeType.Program, end, in statements )
        {
        }
    }

}
