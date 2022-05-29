namespace YantraJS.Core.FastParser
{
    public class AstProgram : AstBlock
    {
        public readonly bool IsAsync;

        public AstProgram(
            FastToken token,
            FastToken end,
            IFastEnumerable<AstStatement> statements,
            bool isAsync)
            : base(token, FastNodeType.Program, end, statements )
        {
            this.IsAsync = isAsync;
        }
    }

}
