using System;

namespace YantraJS.Core;


[Flags]
public enum JSValueType: byte
{
    Undefined = 1,
    Null = 3,

    NullOrUndefined = Null | Undefined,

    Boolean = 4,
    Number = 8,
    String = 16,
    Spread = 24,
    Symbol = 40,
    BigInt = 48,
    Decimal = 56,

    Object = 128,
    Function = 136,
    Array = 168,
} 