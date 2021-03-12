namespace YantraJS.Core.FastParser
{
    public abstract class FastStatement: FastNode
    {
        protected FastStatement(FastNode parent, FastNodeType type): base(parent, type, true)
        {

        }
    }
}
