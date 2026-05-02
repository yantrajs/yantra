namespace YantraJS.Core;


public enum JSValueType: byte
{
    Unknown = 0,
    Undefined,
    Null,
    Boolean,
    Number,
    String,
    Object,
    Array,
    Spread,
    Function,
    Symbol,
    BigInt,
    Decimal
} 