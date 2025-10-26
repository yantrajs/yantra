using System.Runtime.CompilerServices;

namespace YantraJS.Core.FastParser
{
    public static class HiddenTokens
    {
        public const int Operator = 0b0100000;
    }

    public static class TokenTypesExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOperator(this TokenTypes type)
        {
            return type > TokenTypes.BeginOperators && type < TokenTypes.EndOperators;
        }

    }

    /// <summary>
    /// Tokens should be arranged in order of precedence
    /// </summary>
    public enum TokenTypes
    {
        Empty = 0,
        // Operator       = 0b0100000,
        // AssignOperator = 0b1000000,
        SquareBracketStart,
        SquareBracketEnd,
        CurlyBracketStart,
        CurlyBracketEnd,
        TemplateBegin,
        TemplatePart,
        TemplateEnd,
        LineTerminator,
        None,
        SemiColon,
        EOF,
        Identifier,
        Number,
        BigInt,
        Decimal,
        String,
        BracketEnd,
        BracketStart,
        Lambda,

        //not used
        BeginAssignTokens,

        Assign,
        AssignMultiply,
        AssignDivide,
        AssignMod,
        AssignAdd,
        AssignXor,
        AssignSubtract,
        AssignUnsignedRightShift,
        AssignBitwideAnd,
        AssignBitwideOr,
        AssignRightShift,
        AssignLeftShift,
        AssignCoalesce,
        AssignPower,

        // not used...
        EndAssignTokens,

        BeginOperators,

        Negate,
        Power,
        Multiply,
        Divide,
        Mod,
        Plus,
        Minus,
        LeftShift,
        RightShift,
        UnsignedRightShift,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        In,
        InstanceOf,
        Equal,
        NotEqual,
        StrictlyEqual,
        StrictlyNotEqual,
        Coalesce,
        BitwiseAnd,
        Xor,
        BitwiseOr,
        BooleanAnd,
        BooleanOr,
        QuestionMark,
        Colon,
        BitwiseNot,
        QuestionDot,
        Dot,
        TripleDots,

        EndOperators,

        Increment,
        Decrement,

        Comma,

        Null,
        False,
        True,
        Hash,
        RegExLiteral,
        OptionalCall,
        OptionalIndex,
    }
}
