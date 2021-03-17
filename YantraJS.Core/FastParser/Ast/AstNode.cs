namespace YantraJS.Core.FastParser
{
    public class AstNode
    {
        public readonly FastNodeType Type;
        public readonly FastToken Start;
        public readonly FastToken End;

        public readonly bool IsStatement;

        public readonly bool IsBinding;

        public AstNode(FastToken start, FastNodeType type, FastToken end, bool isStatement = false, bool isBinding = false)
        {
            this.Start = start;
            this.Type = type;
            this.End = end;
            this.IsStatement = isStatement;
            this.IsBinding = isBinding;
        }

    }

}
