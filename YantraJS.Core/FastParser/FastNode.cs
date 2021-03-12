namespace YantraJS.Core.FastParser
{
    public abstract class FastNode
    {
        public readonly FastNodeType NodeType;

        public readonly bool IsStatement;

        public readonly FastNode Parent;

        public readonly SpanLocation Start;
        public readonly SpanLocation End;

        protected FastNode(
            FastNode parent, 
            FastNodeType nodeType, 
            FastTokenStream stream,
            bool isStatement = false)
        {
            this.Parent = parent;
            this.NodeType = nodeType;
            this.IsStatement = isStatement;
            this.Start = stream.Current.StartLocation;
            Read(stream);
            this.End = stream.Previous.EndLocation;
        }

        internal abstract void Read(FastTokenStream stream);
    }
}
