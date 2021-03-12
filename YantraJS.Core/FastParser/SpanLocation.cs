namespace YantraJS.Core.FastParser
{
    public readonly struct SpanLocation
    {
        public readonly int Line;
        public readonly int Column;

        public SpanLocation(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        public override string ToString()
        {
            return $"{Line}, {Column}";
        }
    }
}
