namespace YantraJS.Core.FastParser
{
    public class FastNode
    {
        public readonly FastNodeType NodeType;

        public readonly bool IsStatement;

        protected FastNode(FastNodeType nodeType, bool isStatement = false)
        {
            this.NodeType = nodeType;
        }
    }
}
