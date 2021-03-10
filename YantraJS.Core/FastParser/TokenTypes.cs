namespace YantraJS.Core.FastParser
{
    public enum TokenTypes
    {
        Empty = 0,

        Identifier,
        Number,
        String,
        EOF,
        BracketEnd,
        BracketStart,
        StrictlyEqual,
        Equal,
        Lambda,
        Assign
    }
}
