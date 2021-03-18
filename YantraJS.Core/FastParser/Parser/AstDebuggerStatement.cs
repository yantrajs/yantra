namespace YantraJS.Core.FastParser
{
    internal class AstDebuggerStatement : AstStatement
    {

        public AstDebuggerStatement(FastToken token):
            base(token, FastNodeType.DebuggerStatement, token)
        {
        }
    }
}