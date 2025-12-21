namespace YantraJS.Core.Interpreter;

public readonly struct JSInstruction
{   
    public readonly string Label;

    public readonly JSIL Code;

    public readonly string ArgS;

    public readonly uint ArgUint;

    public readonly KeyString ArgKey;

    public readonly int ArgInt;
}
