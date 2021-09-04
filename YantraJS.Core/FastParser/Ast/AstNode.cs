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

        //public (int Start, int End) Range =>
        //    (Start.Span.Offset, End.Span.Offset + End.Span.Length);

        //public (int Start, int End) Location =>
        //    (Start.Span.Offset, Start.Span.Offset + Start.Span.Length);

        public StringSpan Code
        {
            get
            {
                var startSpan = this.Start.Span;
                var start = startSpan.Offset;

                if(this.End.Type == TokenTypes.EOF)
                {
                    return startSpan;
                }

                var endSpan = this.End.Span;
                var end = endSpan.Offset;
                var length = endSpan.Length;

                var total = end + length - start;

                return new StringSpan(startSpan.Source,
                    start,
                    total);
            }
        }

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
