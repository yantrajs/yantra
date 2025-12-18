using System.Collections.Generic;

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

public class JSILBlock
{
    private readonly JSILBlock parent;

    JSInstruction[] Instructions;

    SAUint32Map<JSValue> Variables;

    Stack<JSValue> Stack = new Stack<JSValue>();

    public JSILBlock(JSILBlock parent = null)
    {
        this.parent = parent;
    }

    public JSValue Run()
    {
        var pointer = 0;
        for(; ;) {
            ref var current = ref this.Instructions[pointer++];
            switch (current.Code)
            {
                case JSIL.None:
                    continue;
                case JSIL.Load:
                    Stack.Push(Variables[current.ArgKey.Key]);
                    continue;
                case JSIL.Stor:
                    Variables.Put(current.ArgKey.Key) = Stack.Pop();
                    continue;
                case JSIL.Jump:
                    pointer = current.ArgInt;
                    continue;
                case JSIL.JmpT:
                    if(Stack.Pop().BooleanValue)
                    {
                        pointer = current.ArgInt;
                    }
                    continue;
                case JSIL.JmpF:
                    if (!Stack.Pop().BooleanValue)
                    {
                        pointer = current.ArgInt;
                    }
                    continue;
                case JSIL.Colc:
                    var v1 = Stack.Pop();
                    var v2 = Stack.Pop();
                    if (v1 != JSUndefined.Value)
                    {
                        Stack.Push(v1);
                    } else
                    {
                        Stack.Push(v2);
                    }
                    continue;
                case JSIL.StrM:
                    {
                        var target = Stack.Pop();
                        Stack.Push(target[current.ArgS]);
                    }
                    continue;
                case JSIL.KeyM:
                    {
                        var target = Stack.Pop();
                        Stack.Push(target[current.ArgKey]);
                    }
                    continue;
                case JSIL.IntM:
                    {
                        var target = Stack.Pop();
                        Stack.Push(target[current.ArgUint]);
                    }
                    continue;
                case JSIL.ValM:
                    {
                        var target = Stack.Pop();
                        var symbol = Stack.Pop();
                        Stack.Push(target[symbol]);
                    }
                    continue;
                case JSIL.Inv0:
                    {
                        var target = Stack.Pop();
                        Stack.Push( target.Call());
                    }
                    continue;
                case JSIL.Inv1:
                    {
                        var target = Stack.Pop();
                        var arg0 = Stack.Pop();
                        Stack.Push(target.Call(JSUndefined.Value, arg0));
                    }
                    continue;
                case JSIL.Inv2:
                    {
                        var target = Stack.Pop();
                        var arg0 = Stack.Pop();
                        var arg1 = Stack.Pop();
                        Stack.Push(target.Call(JSUndefined.Value, arg0, arg1));
                    }
                    continue;
                case JSIL.Inv3:
                    {
                        var target = Stack.Pop();
                        var arg0 = Stack.Pop();
                        var arg1 = Stack.Pop();
                        var arg2 = Stack.Pop();
                        Stack.Push(target.Call(JSUndefined.Value, arg0, arg1, arg2));
                    }
                    continue;
                case JSIL.Inv4:
                    {
                        var target = Stack.Pop();
                        var arg0 = Stack.Pop();
                        var arg1 = Stack.Pop();
                        var arg2 = Stack.Pop();
                        var arg3 = Stack.Pop();
                        Stack.Push(target.Call(JSUndefined.Value, arg0, arg1, arg3));
                    }
                    continue;
                case JSIL.InvN:
                    {
                        var target = Stack.Pop();
                        var argList = new JSValue[current.ArgInt];
                        for(int i = 0; i < current.ArgInt;i++)
                        {
                            argList[i] = Stack.Pop();
                        }
                        Stack.Push(target.Call(JSUndefined.Value, argList));
                    }
                    continue;
                case JSIL.Met0:
                    continue;
                case JSIL.Met1:
                    continue;
                case JSIL.Met2:
                    continue;
                case JSIL.Met3:
                    continue;
                case JSIL.Met4:
                    continue;
                case JSIL.MetN:
                    continue;
                case JSIL.Add:
                    continue;
                case JSIL.Mul:
                    continue;
                case JSIL.Div:
                    continue;
                case JSIL.Mod:
                    continue;
                case JSIL.And:
                    continue;
                case JSIL.Or:
                    continue;
                case JSIL.Not:
                    continue;
                case JSIL.Clr0:
                    continue;
                case JSIL.RetU:
                    continue;
                case JSIL.RetV:
                    continue;
                case JSIL.Yild:
                    continue;
                case JSIL.Thro:
                    continue;
                case JSIL.BegT:
                    continue;
                case JSIL.EndT:
                    continue;
                case JSIL.BegC:
                    continue;
                case JSIL.EndC:
                    continue;
                case JSIL.BegF:
                    continue;
                case JSIL.EndF:
                    continue;
            }
        }
    }

}
