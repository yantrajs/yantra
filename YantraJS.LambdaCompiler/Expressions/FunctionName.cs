#nullable enable

namespace YantraJS.Expressions
{
    public readonly struct FunctionName
    {
        public readonly string Name;
        public readonly string? Location;
        public readonly int Line;
        public readonly int Column;

        public string FullName =>
            $"{Name}-{Location}:{Line},{Column}";

        public FunctionName(string? name, string? location = null, int line = 0, int column = 0)
        {
            this.Name = name ?? "Unnamed";
            this.Location = location;
            this.Line = line;
            this.Column = column;
        }

        public static implicit operator FunctionName(string name)
        {
            return new FunctionName(name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}