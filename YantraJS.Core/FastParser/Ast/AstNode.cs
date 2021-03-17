namespace YantraJS.Core.FastParser
{
    public class AstNode
    {
        public readonly FastNodeType Type;
        public readonly FastToken Start;
        public readonly FastToken End;

        public readonly bool IsStatement;

        public AstNode(FastToken start, FastNodeType type, FastToken end)
        {
            this.Start = start;
            this.Type = type;
            this.End = end;
        }

        public AstNode(FastToken start, FastNodeType type, FastToken end, bool isStatement)
        {
            this.Start = start;
            this.Type = type;
            this.End = end;
            this.IsStatement = isStatement;
        }

    }

}
