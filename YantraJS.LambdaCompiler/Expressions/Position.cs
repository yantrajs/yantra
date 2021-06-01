#nullable enable

namespace YantraJS.Expressions
{
    public readonly struct Position
    {
        public readonly int Line;
        public readonly int Column;

        public Position(int line, int column)
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
