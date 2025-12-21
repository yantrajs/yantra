using System.Collections.Generic;

namespace YantraJS.Core.Interpreter;

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
                case JSIL.LdSE:
                    {
                        var target = Stack.Pop();
                        Stack.Push(target[current.ArgS]);
                    }
                    continue;
                case JSIL.LdKE:
                    {
                        var target = Stack.Pop();
                        Stack.Push(target[current.ArgKey]);
                    }
                    continue;
                case JSIL.LdIE:
                    {
                        var target = Stack.Pop();
                        Stack.Push(target[current.ArgUint]);
                    }
                    continue;
                case JSIL.LdVE:
                    {
                        var target = Stack.Pop();
                        var symbol = Stack.Pop();
                        Stack.Push(target[symbol]);
                    }
                    continue;
                case JSIL.StSE:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        target[current.ArgS] = arg;
                        Stack.Push(arg);
                    }
                    continue;
                case JSIL.StKE:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        target[current.ArgKey] = arg;
                        Stack.Push(arg);
                    }
                    continue;
                case JSIL.StIE:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        target[current.ArgUint] = arg;
                        Stack.Push(arg);
                    }
                    continue;
                case JSIL.StVE:
                    {
                        var target = Stack.Pop();
                        var name = Stack.Pop();
                        var arg = Stack.Pop();
                        target[name] = arg;
                        Stack.Push(arg);
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
                case JSIL.InvS:
                    {
                        var target = Stack.Pop();
                        var argList = new Sequence<JSValue>();
                        for (int i = 0; i < current.ArgInt; i++)
                        {
                            argList[i] = Stack.Pop();
                        }
                        var spread = Stack.Pop();
                        var e = spread.GetElementEnumerator();
                        while(e.MoveNextOrDefault(out var v, JSUndefined.Value))
                        {
                            argList.Add(v);
                        }
                        Stack.Push(target.Call(JSUndefined.Value, argList.ToArray()));
                    }
                    continue;
                case JSIL.MetK0:
                    {
                        var target = Stack.Pop();
                        Stack.Push(target.InvokeMethod(in current.ArgKey));
                    }
                    continue;
                case JSIL.MetK1:
                    {
                        var target = Stack.Pop();
                        var arg0 = Stack.Pop();
                        Stack.Push(target.InvokeMethod(in current.ArgKey, arg0));
                    }
                    continue;
                case JSIL.MetK2:
                    {
                        var target = Stack.Pop();
                        var arg0 = Stack.Pop();
                        var arg1 = Stack.Pop();
                        Stack.Push(target.InvokeMethod(in current.ArgKey, arg0, arg1));
                    }
                    continue;
                case JSIL.MetK3:
                    {
                        var target = Stack.Pop();
                        var arg0 = Stack.Pop();
                        var arg1 = Stack.Pop();
                        var arg2 = Stack.Pop();
                        Stack.Push(target.InvokeMethod(in current.ArgKey, arg0, arg1, arg2));
                    }
                    continue;
                case JSIL.MetK4:
                    {
                        var target = Stack.Pop();
                        var arg0 = Stack.Pop();
                        var arg1 = Stack.Pop();
                        var arg2 = Stack.Pop();
                        var arg3 = Stack.Pop();
                        Stack.Push(target.InvokeMethod(in current.ArgKey, arg0, arg1, arg2, arg3));
                    }
                    continue;
                case JSIL.MetKN:
                    {
                        var target = Stack.Pop();
                        var argList = new JSValue[current.ArgInt];
                        for (int i = 0; i < current.ArgInt; i++)
                        {
                            argList[i] = Stack.Pop();
                        }
                        Stack.Push(target.InvokeMethod(in current.ArgKey, argList));
                    }
                    continue;
                case JSIL.MetKS:
                    {
                        var target = Stack.Pop();
                        var argList = new Sequence<JSValue>();
                        for (int i = 0; i < current.ArgInt; i++)
                        {
                            argList[i] = Stack.Pop();
                        }
                        var spread = Stack.Pop();
                        var e = spread.GetElementEnumerator();
                        while (e.MoveNextOrDefault(out var v, JSUndefined.Value))
                        {
                            argList.Add(v);
                        }
                        Stack.Push(target.InvokeMethod(current.ArgKey, argList.ToArray()));
                    }
                    continue;
                case JSIL.MetV0:
                    {
                        var target = Stack.Pop();
                        var name = Stack.Pop();
                        Stack.Push(target.InvokeMethod(name));
                    }
                    continue;
                case JSIL.MetV1:
                    {
                        var target = Stack.Pop();
                        var name = Stack.Pop();
                        var arg0 = Stack.Pop();
                        Stack.Push(target.InvokeMethod(name, arg0));
                    }
                    continue;
                case JSIL.MetV2:
                    {
                        var target = Stack.Pop();
                        var name = Stack.Pop();
                        var arg0 = Stack.Pop();
                        var arg1 = Stack.Pop();
                        Stack.Push(target.InvokeMethod(name, arg0, arg1));
                    }
                    continue;
                case JSIL.MetV3:
                    {
                        var target = Stack.Pop();
                        var name = Stack.Pop();
                        var arg0 = Stack.Pop();
                        var arg1 = Stack.Pop();
                        var arg2 = Stack.Pop();
                        Stack.Push(target.InvokeMethod(name, arg0, arg1, arg2));
                    }
                    continue;
                case JSIL.MetV4:
                    {
                        var target = Stack.Pop();
                        var name = Stack.Pop();
                        var arg0 = Stack.Pop();
                        var arg1 = Stack.Pop();
                        var arg2 = Stack.Pop();
                        var arg3 = Stack.Pop();
                        Stack.Push(target.InvokeMethod(name, arg0, arg1, arg2, arg3));
                    }
                    continue;
                case JSIL.MetVN:
                    {
                        var target = Stack.Pop();
                        var name = Stack.Pop();
                        var argList = new JSValue[current.ArgInt];
                        for (int i = 0; i < current.ArgInt; i++)
                        {
                            argList[i] = Stack.Pop();
                        }
                        Stack.Push(target.InvokeMethod(name, argList));
                    }
                    continue;
                case JSIL.MetVS:
                    {
                        var target = Stack.Pop();
                        var name = Stack.Pop();
                        var argList = new Sequence<JSValue>();
                        for (int i = 0; i < current.ArgInt; i++)
                        {
                            argList[i] = Stack.Pop();
                        }
                        var spread = Stack.Pop();
                        var e = spread.GetElementEnumerator();
                        while (e.MoveNextOrDefault(out var v, JSUndefined.Value))
                        {
                            argList.Add(v);
                        }
                        Stack.Push(target.InvokeMethod(name, argList.ToArray()));
                    }
                    continue;
                case JSIL.Add:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        Stack.Push(target.AddValue(arg));
                    }
                    continue;
                case JSIL.Mul:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        Stack.Push(target.Multiply(arg));
                    }
                    continue;
                case JSIL.Div:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        Stack.Push(target.Divide(arg));
                    }
                    continue;
                case JSIL.Mod:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        Stack.Push(target.Modulo(arg));
                    }
                    continue;
                case JSIL.BAnd:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        Stack.Push(target.BitwiseAnd(arg));
                    }
                    continue;
                case JSIL.BOr:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        Stack.Push(target.BitwiseOr(arg));
                    }
                    continue;
                case JSIL.BXor:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        Stack.Push(target.BitwiseXor(arg));
                    }
                    continue;
                case JSIL.BNot:
                    {
                        var target = Stack.Pop();
                        Stack.Push(target.Negate());
                    }
                    continue;
                case JSIL.Not:
                    {
                        var target = Stack.Pop();
                        Stack.Push(!target.BooleanValue ? JSBoolean.True : JSBoolean.False);
                    }
                    continue;
                case JSIL.Clr0:
                    continue;
                case JSIL.RetU:
                    return JSUndefined.Value;
                    // continue;
                case JSIL.RetV:
                    return Stack.Pop();
                    // continue;
                case JSIL.Yild:
                    return Stack.Pop();
                    // continue;
                case JSIL.Thro:
                    throw JSException.FromValue(Stack.Pop());
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
