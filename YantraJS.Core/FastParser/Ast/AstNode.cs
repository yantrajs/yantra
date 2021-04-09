using YantraJS.Utils;

namespace YantraJS.Core.FastParser
{
    public abstract class AstNode
    {
        public readonly FastNodeType Type;
        public readonly FastToken Start;
        public readonly FastToken End;

        public readonly bool IsStatement;

        public readonly bool IsBinding;

        public (int Start, int End) Range =>
            (Start.Span.Offset, End.Span.Offset + End.Span.Length);

        public (int Start, int End) Location =>
            (Start.Span.Offset, Start.Span.Offset + Start.Span.Length);

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
