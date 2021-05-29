#nullable enable

namespace YantraJS.Expressions
{
    public enum YExpressionType
    {
        Binary,
        Constant,
        Conditional,
        Assign,
        Parameter,
        Block,
        Call,
        New,
        Field,
        Property,
        NewArray,
        GoTo,
        Return,
        Loop,
        TypeAs,
        Lambda,
        Label,
        TypeIs,
        NewArrayBounds,
        ArrayIndex,
        Index,
        Coalesce,
        Unary,
        ArrayLength,
        TypeEqual,
        TryCatchFinally,
        Throw,
        Convert,
        Invoke,
        Delegate,
        MemberInit,
        Relay,
        Empty,
        Switch,
        Yield
    }
}
