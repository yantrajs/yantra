namespace YantraJS.Core.FastParser
{
    public abstract class FastNode
    {
        public readonly FastNodeType NodeType;

        public readonly bool IsStatement;

        public readonly FastNode Parent;

        protected FastNode(FastNode parent, FastNodeType nodeType, bool isStatement = false)
        {
            this.Parent = parent;
            this.NodeType = nodeType;
        }

        internal abstract void Read(FastTokenStream stream);
    }
}
