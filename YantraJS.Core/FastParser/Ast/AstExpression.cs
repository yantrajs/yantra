namespace YantraJS.Core.FastParser
{
    public class AstExpression : AstNode
    {
        public AstExpression(FastToken start, FastNodeType type, FastToken end, bool isBinding = false)
            : base(start, type, end, false, isBinding)
        {
        }
    }

}
