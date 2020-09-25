using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public readonly struct Arguments
    {

        public static Arguments Empty = new Arguments { };

        // public JSContext Context;

        private const int MinArray = 5;

        public readonly int Length;

        public readonly JSValue This;

        private readonly JSValue Arg0;

        private readonly JSValue Arg1;

        private readonly JSValue Arg2;

        private readonly JSValue Arg3;

        private readonly JSValue[] Args;

        public IEnumerable<JSValue> All
        {
            get
            {
                switch (Length)
                {
                    case 0:
                        yield break;
                    case 1:
                        yield return Arg0;
                        yield break;
                    case 2:
                        yield return Arg0;
                        yield return Arg1;
                        yield break;
                    case 3:
                        yield return Arg0;
                        yield return Arg1;
                        yield return Arg2;
                        yield break;
                    case 4:
                        yield return Arg0;
                        yield return Arg1;
                        yield return Arg2;
                        yield return Arg3;
                        yield break;
                    default:
                        foreach (var a in Args)
                            yield return a;
                        yield break;
                }
            }
        }

        public Arguments CopyForCall()
        {
            switch(Length)
            {
                case 0:
                    return new Arguments(JSUndefined.Value);
                case 1:
                    return new Arguments(Arg0);
                case 2:
                    return new Arguments(Arg0, Arg1);
                case 3:
                    return new Arguments(Arg0, Arg1, Arg2);
                case 4:
                    return new Arguments(Arg0, Arg1, Arg2, Arg3);
                case 5:
                    return new Arguments(Args[0], Args[1], Args[2], Args[3], Args[4]);
                default:
                    var sa = new JSValue[Length - 1];
                    Array.Copy(Args, 1, sa, 0, sa.Length);
                    return new Arguments(Args[0], sa);
            }
        }

        public Arguments CopyForApply()
        {

            // in apply first parameter is @this and rest is An Array
            var (@this, args) = Get2();
            if (!(args is JSArray argArray))
                return new Arguments(@this);
            switch(argArray._length)
            {
                case 0:
                    return new Arguments(@this);
                case 1:
                    return new Arguments(@this, argArray[0]);
                case 2:
                    return new Arguments(@this, argArray[0], argArray[1]);
                case 3:
                    return new Arguments(@this, argArray[0], argArray[1], argArray[2]);
                case 4:
                    return new Arguments(@this, argArray[0], argArray[1], argArray[2], argArray[3]);
                default:
                    return new Arguments(@this, argArray);
            }
        }

        public Arguments(JSValue @this)
        {
            This = @this;
            Length = 0;
            Arg0 = null;
            Arg1 = null;
            Arg2 = null;
            Arg3 = null;
            Args = null;
        }

        public Arguments(JSValue @this, JSValue a0)
        {
            This = @this;
            Length = 1;
            Arg0 = a0;
            Arg1 = null;
            Arg2 = null;
            Arg3 = null;
            Args = null;
        }

        public Arguments(JSValue @this, JSValue a0, JSValue a1)
        {
            This = @this;
            Length = 2;
            Arg0 = a0;
            Arg1 = a1;
            Arg2 = null;
            Arg3 = null;
            Args = null;
        }


        public Arguments(JSValue @this, JSValue a0, JSValue a1, JSValue a2)
        {
            This = @this;
            Length = 3;
            Arg0 = a0;
            Arg1 = a1;
            Arg2 = a2;
            Arg3 = null;
            Args = null;
        }

        public Arguments(JSValue @this, JSValue a0, JSValue a1, JSValue a2, JSValue a3)
        {
            This = @this;
            Length = 4;
            Arg0 = a0;
            Arg1 = a1;
            Arg2 = a2;
            Arg3 = a3;
            Args = null;
        }

        public Arguments(JSValue @this, JSValue[] args)
        {
            This = @this;
            Length = args.Length;
            switch(Length)
            {
                case 0:
                    Arg0 = null;
                    Arg1 = null;
                    Arg2 = null;
                    Arg3 = null;
                    Args = null;
                    break;
                case 1:
                    Arg0 = args[0];
                    Arg1 = null;
                    Arg2 = null;
                    Arg3 = null;
                    Args = null;
                    break;
                case 2:
                    Arg0 = args[0];
                    Arg1 = args[1];
                    Arg2 = null;
                    Arg3 = null;
                    Args = null;
                    break;
                case 3:
                    Arg0 = args[0];
                    Arg1 = args[1];
                    Arg2 = args[2];
                    Arg3 = null;
                    Args = null;
                    break;
                case 4:
                    Arg0 = args[0];
                    Arg1 = args[1];
                    Arg2 = args[2];
                    Arg3 = args[3];
                    Args = null;
                    break;
                default:
                    Arg0 = null;
                    Arg1 = null;
                    Arg2 = null;
                    Arg3 = null;
                    Args = args;
                    break;
            }
        }

        private Arguments(JSValue @this, Arguments src)
        {
            Length = src.Length;
            Arg0 = src.Arg0;
            Arg1 = src.Arg1;
            Arg2 = src.Arg2;
            Arg3 = src.Arg3;
            Args = src.Args;
            This = @this;
        }

        public Arguments OverrideThis(JSValue @this)
        {
            return new Arguments(@this, this);
        }

        public JSValue Get1()
        {
            if (Length == 0)
                return JSUndefined.Value;
            if (Length < MinArray)
                return Arg0;
            return Args[0];
        }

        public (JSValue, JSValue) Get2()
        {
            if (Length == 0)
                return (JSUndefined.Value, JSUndefined.Value);
            if (Length == 1)
                return (Arg0, JSUndefined.Value);
            if (Length < MinArray)
                return (Arg0, Arg1);
            return (Args[0], Args[1]);
        }

        public (JSValue, JSValue, JSValue) Get3()
        {
            if (Length == 0)
                return (JSUndefined.Value, JSUndefined.Value, JSUndefined.Value);
            if (Length == 1)
                return (Arg0, JSUndefined.Value, JSUndefined.Value);
            if (Length == 2)
                return (Arg0, Arg1, JSUndefined.Value);
            if (Length < MinArray)
                return (Arg0, Arg1, Arg2);
            return (Args[0], Args[1], Args[2]);
        }

        public (JSValue, JSValue, JSValue, JSValue) Get4()
        {
            if (Length == 0)
                return (JSUndefined.Value, JSUndefined.Value, JSUndefined.Value, JSUndefined.Value);
            if (Length == 1)
                return (Arg0, JSUndefined.Value, JSUndefined.Value, JSUndefined.Value);
            if (Length == 2)
                return (Arg0, Arg1, JSUndefined.Value, JSUndefined.Value);
            if (Length == 3)
                return (Arg0, Arg1, Arg2, JSUndefined.Value);
            if (Length < MinArray)
                return (Arg0, Arg1, Arg2, Arg3);
            return (Args[0], Args[1], Args[2], Args[3]);
        }

        public JSValue[] GetArgs()
        {
            return Args;
        }

        public bool TryGetAt(int index, out JSValue a)
        {
            if (Length > index)
            {
                switch(index)
                {
                    case 0:
                        a = Arg0;
                        break;
                    case 1:
                        a = Arg1;
                        break;
                    case 2:
                        a = Arg2;
                        break;
                    case 3:
                        a = Arg3;
                        break;
                    default:
                        a = Args[index];
                        break;
                }
                return true;
            }
            a = null;
            return false;
        }
    }
}
