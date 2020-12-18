using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace YantraJS.Core
{

    [StructLayout(LayoutKind.Sequential)]
    public readonly partial struct Arguments
    {

        public static Arguments Empty = new Arguments { };

        private const int MinArray = 5;

        public readonly int Length;

        public readonly JSValue This;

        // public readonly JSValue NewTarget;

        private readonly JSValue Arg0;

        private readonly JSValue Arg1;

        private readonly JSValue Arg2;

        private readonly JSValue Arg3;

        private readonly JSValue[] Args;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arguments CopyForApply()
        {

            // in apply first parameter is @this and rest is An Array
            var (@this, args) = Get2();
            if (args is JSArguments arg) {
                ref var argList = ref arg.arguments;
                switch(argList.Length)
                {
                    case 0:
                        return new Arguments(@this);
                    case 1:
                        return new Arguments(@this, argList.Arg0);
                    case 2:
                        return new Arguments(@this, argList.Arg0, argList.Arg1);
                    case 3:
                        return new Arguments(@this, argList.Arg0, argList.Arg1, argList.Arg2);
                    case 4:
                        return new Arguments(@this, argList.Arg0, argList.Arg1, argList.Arg2, argList.Arg3);
                    default:
                        return new Arguments(@this, argList.Args);

                }
            }
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arguments(JSValue @this, JSValue[] list, int length)
        {
            This = @this;
            Length = length;
            JSValue[] args = new JSValue[length];
            int i = 0;
            foreach(var a in list)
            {
                if (a.IsSpread)
                {
                    for (uint j = 0; j < a.Length; j++,i++)
                    {
                        args[i] = a[j];
                    }
                    continue;
                }
                list[i] = a;
            }
            switch (Length)
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arguments OverrideThis(JSValue @this)
        {
            return new Arguments(@this, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSValue Get1()
        {
            if (Length == 0)
                return JSUndefined.Value;
            if (Length < MinArray)
                return Arg0;
            return Args[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (JSValue, JSValue) Get2(JSValue def1, JSValue def2)
        {
            if (Length == 0)
                return (def1, def2);
            if (Length == 1)
                return (Arg0, def2);
            if (Length < MinArray)
                return (Arg0, Arg1);
            return (Args[0], Args[1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int, int, int, int, int, int, int) Get7Int()
        {
            if (Length == 0)
                return (0, 0, 0, 0, 0, 0, 0);
            if (Length == 1)
                return (Arg0.IntValue, 0, 0, 0, 0, 0, 0);
            if (Length == 2)
                return (Arg0.IntValue, Arg1.IntValue, 0, 0, 0, 0, 0);
            if (Length == 3)
                return (Arg0.IntValue, Arg1.IntValue, Arg2.IntValue, 0, 0, 0, 0);
            if (Length == 4)
                return (Arg0.IntValue, Arg1.IntValue, Arg2.IntValue, Arg3.IntValue, 0, 0, 0);
            if (Length == 5)
                return (Args[0].IntValue, Args[1].IntValue, Args[2].IntValue, Args[3].IntValue, Args[4].IntValue, 0, 0);
            if (Length == 6)
                return (Args[0].IntValue, Args[1].IntValue, Args[2].IntValue, Args[3].IntValue, Args[4].IntValue, Args[5].IntValue,0);
           
           return (Args[0].IntValue, Args[1].IntValue, Args[2].IntValue, Args[3].IntValue, Args[4].IntValue, Args[5].IntValue, Args[6].IntValue);

           
        }


        public JSValue[] GetArgs()
        {
            return Args;
        }

        static readonly JSValue[] _Empty = new JSValue[] { };

        public JSValue[] ToArray()
        {
            switch(Length)
            {
                case 0:
                    return _Empty;
                case 1:
                    return new JSValue[] { Arg0 };
                case 2:
                    return new JSValue[] { Arg0, Arg1 };
                case 3:
                    return new JSValue[] { Arg0, Arg1, Arg2 };
                case 4:
                    return new JSValue[] { Arg0, Arg1, Arg2, Arg3 };
            }
            return Args;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIntAt(int index, int def)
        {
            if (Length > index)
            {
                switch (index)
                {
                    case 0:
                        return Arg0.IntValue;
                    case 1:
                        return Arg1.IntValue;
                    case 2:
                        return Arg2.IntValue;
                    case 3:
                        return Arg3.IntValue;
                    default:
                        return Args[index].IntValue;
                }
            }
            return def;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetDoubleAt(int index, double def)
        {
            if (Length > index)
            {
                switch (index)
                {
                    case 0:
                        return Arg0.DoubleValue;
                    case 1:
                        return Arg1.DoubleValue;
                    case 2:
                        return Arg2.DoubleValue;
                    case 3:
                        return Arg3.DoubleValue;
                    default:
                        return Args[index].DoubleValue;
                }
            }
            return def;
        }



        internal JSValue this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (Length > index)
                {
                    if (Length >= MinArray)
                        return Args[index];
                    switch (index)
                    {
                        case 0:
                            return Arg0;
                        case 1:
                            return Arg1;
                        case 2:
                            return Arg2;
                        case 3:
                            return Arg3;
                        default:
                            return Args[index];
                    }
                }
                return JSUndefined.Value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSValue GetAt(int index)
        {
            if (Length >= MinArray)
                return index < Length ? Args[index] : JSUndefined.Value;
            switch (index)
            {
                case 0:
                    return Arg0 ?? JSUndefined.Value;
                case 1:
                    return Arg1 ?? JSUndefined.Value;
                case 2:
                    return Arg2 ?? JSUndefined.Value;
                case 3:
                    return Arg3 ?? JSUndefined.Value;
                default:
                    return index >= Length ? JSUndefined.Value : Args[index];
            }
        }

        public JSValue RestFrom(uint index)
        {
            var a = new JSArray();
            ref var ae = ref a.GetElements(true);
            for (uint i = index; i < Length; i++)
            {
                ae[(uint)i] = JSProperty.Property( GetAt((int)i));
            }
            a._length = (uint)Length - index;
            return a;
        }


        internal IElementEnumerator GetElementEnumerator()
        {
            return new ArgumentsElementEnumerator(this);
        }

        struct ArgumentsElementEnumerator: IElementEnumerator
        {
            readonly Arguments arguments;
            private int index;

            public ArgumentsElementEnumerator(Arguments arguments)
            {
                this.arguments = arguments;
                this.index = -1;
            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                if((++this.index ) > arguments.Length)
                {
                    index = (uint)this.index;
                    value = arguments.GetAt(this.index);
                    hasValue = true;
                    return true;
                }
                index = 0;
                value = JSUndefined.Value;
                hasValue = false;
                return false;
            }

            public bool MoveNext(out JSValue value)
            {
                if ((++this.index) > arguments.Length)
                {
                    value = arguments.GetAt(this.index);
                    return true;
                }
                value = JSUndefined.Value;
                return false;
            }
        }
    }
}
