namespace YantraJS.Core.FastParser
{

    public abstract class FastNode
    {
        public readonly FastNodeType NodeType;

        public readonly bool IsStatement;

        public readonly FastNode Parent;

        public readonly IVariableScope VariableScope;

        public readonly SpanLocation Start;
        public readonly SpanLocation End;

        protected FastNode(
            FastNode parent, 
            FastNodeType nodeType, 
            FastTokenStream stream,
            bool isStatement = false)
        {
            this.Parent = parent;
            this.VariableScope = parent is IVariableScope ivar ? ivar : parent.VariableScope;
            this.NodeType = nodeType;
            this.IsStatement = isStatement;
            if (stream != null)
            {
                this.Start = stream.Current.StartLocation;
                Read(stream);
                this.End = stream.Previous.EndLocation;
            }
        }

        internal abstract void Read(FastTokenStream stream);
    }
}
