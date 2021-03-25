namespace YantraJS.Core.FastParser
{
    public class AstDebuggerStatement : AstStatement
    {

        public AstDebuggerStatement(FastToken token):
            base(token, FastNodeType.DebuggerStatement, token)
        {
        }
    }
}