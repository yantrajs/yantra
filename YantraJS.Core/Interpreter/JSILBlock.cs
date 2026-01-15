using System;
using System.Collections.Generic;

namespace YantraJS.Core.Interpreter;

public class JSILBlock
{
    private readonly JSILBlock parent;

    JSInstruction[] Instructions;

    SAUint32Map<JSValue> Variables;

    Stack<JSValue> Stack = new Stack<JSValue>();
    private int last;

    public JSILBlock(JSILBlock parent = null)
    {
        this.parent = parent;
    }

    public JSValue Run()
    {
        var pointer = this.last;
        for(; ;) {
            ref var current = ref this.Instructions[pointer++];
            switch (current.Code)
            {
                case JSIL.None:
                    continue;
                case JSIL.Pop:
                    Stack.Pop();
                    continue;
                case JSIL.Load:
                    Stack.Push(Variables[current.ArgKey.Key]);
                    continue;
                case JSIL.Stor:
                    Variables.Put(current.ArgKey.Key) = Stack.Pop();
                    continue;
                case JSIL.Dup:
                    Stack.Push(Stack.Peek());
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
                case JSIL.Apd:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        (target as JSArray).Add(arg);
                        Stack.Push(target);
                    }
                    continue;
                case JSIL.ApdS:
                    {
                        var target = Stack.Pop();
                        var arg = Stack.Pop();
                        (target as JSArray).AddRange(arg);
                        Stack.Push(target);
                    }
                    continue;
                case JSIL.NAry:
                    {
                        Stack.Push(new JSArray(current.ArgUint));
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
                case JSIL.Inv:
                    {
                        var target = Stack.Pop();
                        var argList = new Sequence<JSValue>();
                        for (int i = current.ArgInt; i >= 0; i--)
                        {
                            argList[i] = Stack.Pop();
                        }
                        Stack.Push(target.InvokeFunction(Arguments.Spread(JSUndefined.Value, argList)));
                    }
                    continue;
                case JSIL.New:
                    {
                        var target = Stack.Pop();
                        var a = this.CreateArguments(JSUndefined.Value, current.ArgInt);
                        Stack.Push(target.CreateInstance(in a));
                    }
                    continue;
                case JSIL.NewS:
                    {
                        var target = Stack.Pop();
                        var a = this.CreateSpreadArguments(JSUndefined.Value, current.ArgInt);
                        Stack.Push(target.CreateInstance(in a));
                    }
                    continue;
                case JSIL.MetK:
                    {
                        var target = Stack.Pop();
                        var fx = target.GetMethod(in current.ArgKey);
                        if (fx == null)
                            throw JSContext.Current.NewTypeError($"Method {current.ArgKey.Value} not found in {target}");
                        var a = this.CreateArguments(target, current.ArgInt);
                        Stack.Push(fx(in a));
                    }
                    continue;
                case JSIL.MetKS:
                    {
                        var target = Stack.Pop();
                        var fx = target.GetMethod(in current.ArgKey);
                        if (fx == null)
                            throw JSContext.Current.NewTypeError($"Method {current.ArgKey.Value} not found in {target}");
                        var a = this.CreateSpreadArguments(target, current.ArgInt);
                        Stack.Push(fx(in a));
                    }
                    continue;
                case JSIL.MetV:
                    {
                        var target = Stack.Pop();
                        var name = Stack.Pop();
                        var fx = target[name];
                        if (fx == JSUndefined.Value)
                            throw JSContext.Current.NewTypeError($"Method {name} not found in {target}");
                        var a = this.CreateArguments(target, current.ArgInt);
                        Stack.Push(fx.InvokeFunction(in a));
                    }
                    continue;
                case JSIL.MetVS:
                    {
                        var target = Stack.Pop();
                        var name = Stack.Pop();
                        var fx = target[name];
                        if (fx == JSUndefined.Value)
                            throw JSContext.Current.NewTypeError($"Method {name} not found in {target}");
                        var a = this.CreateSpreadArguments(target, current.ArgInt);
                        Stack.Push(fx.InvokeFunction(in a));
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
                    this.last = pointer;
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

    private Arguments CreateArguments(JSValue @this, int argInt)
    {
        JSValue a1, a2, a3, a4;
        switch (argInt)
        {
            case 0:
                return Arguments.Empty;
            case 1:
                return new Arguments(@this, Stack.Pop());
            case 2:
                a2 = Stack.Pop();
                a1 = Stack.Pop();
                return new Arguments(@this, a1, a2);
            case 3:
                a3 = Stack.Pop();
                a2 = Stack.Pop();
                a1 = Stack.Pop();
                return new Arguments(@this, a1, a2, a3);
            case 4:
                a4 = Stack.Pop();
                a3 = Stack.Pop();
                a2 = Stack.Pop();
                a1 = Stack.Pop();
                return new Arguments(@this, a1, a2, a3, a4);
        }

        var argList = new JSValue[argInt];
        for (int i = argInt; i >= 0; i--)
        {
            argList[i] = Stack.Pop();
        }
        return new Arguments(@this, argList);
    }

    private Arguments CreateSpreadArguments(JSValue @this, int argInt)
    {
        var argList = new Sequence<JSValue>(argInt);
        for (int i = argInt; i >= 0; i--)
        {
            argList[i] = Stack.Pop();
        }
        return Arguments.Spread(@this, argList);
    }
}
