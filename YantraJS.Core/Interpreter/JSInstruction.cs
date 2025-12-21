namespace YantraJS.Core.Interpreter;

public readonly struct JSInstruction
{   
    public readonly JSIL Code;

    public readonly string ArgS;

    public readonly uint ArgUint;

    public readonly KeyString ArgKey;

    public readonly int ArgInt;

    public JSInstruction(JSIL code)
    {
        Code = code;
    }

    public JSInstruction(JSIL code, int label)
    {
        Code = code;
        ArgInt = label;
    }

    public JSInstruction(JSIL code, uint uVal)
    {
        Code = code;
        ArgUint = uVal;
    }

}
