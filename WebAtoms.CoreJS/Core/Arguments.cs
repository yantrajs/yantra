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

        private readonly int length;

        public readonly JSValue This;

        private readonly JSValue Arg0;

        private readonly JSValue Arg1;

        private readonly JSValue Arg2;

        private readonly JSValue Arg3;

        private readonly JSValue[] Args;

        public Arguments CopyForCall()
        {
            var a = new Arguments { 
                This = length > 0 ? Arg0 : JSUndefined.Value,
                length = this.length > 0 ? this.length - 1 : 0,
                Arg0 = Arg1,
                Arg1 = Arg2,
                Arg2 = Arg3
            };
            if (this.length == MinArray)
            {
                a.This = Args[0];
                a.Arg0 = Args[1];
                a.Arg1 = Args[2];
                a.Arg2 = Args[3];
                a.Arg3 = Args[4];
                return a;
            }
            if (this.length > MinArray)
            {
                a.This = Args[0];
                a.Args = new JSValue[length - 1];
                Array.Copy(Args, 1,  a.Args, 0, a.Args.Length);
            }
            return a;
        }

        public Arguments CopyForApply()
        {

            // in apply first parameter is @this and rest is An Array
            var (@this, args) = Get2();
            if (!(args is JSArray argArray))
                return New(@this);
            switch(argArray._length)
            {
                case 0:
                    return New(@this);
                case 1:
                    return New(@this, argArray[0]);
                case 2:
                    return New(@this, argArray[0], argArray[1]);
                case 3:
                    return New(@this, argArray[0], argArray[1], argArray[2]);
                case 4:
                    return New(@this, argArray[0], argArray[1], argArray[2], argArray[3]);
                default:
                    return New(@this, argArray);
            }
        }

        public static Arguments New(JSValue @this)
        {
            return new Arguments { 
                This = @this
            };
        }

        public static Arguments New(JSValue @this, JSValue a0)
        {
            return new Arguments
            {
                This = @this,
                length = 1,
                Arg0 = a0
            };
        }

        public static Arguments New(JSValue @this, JSValue a0, JSValue a1)
        {
            return new Arguments
            {
                This = @this,
                length = 2,
                Arg0 = a0,
                Arg1 = a1
            };
        }

        public static Arguments New(JSValue @this, JSValue a0, JSValue a1, JSValue a2)
        {
            return new Arguments
            {
                This = @this,
                length = 3,
                Arg0 = a0,
                Arg1 = a1,
                Arg2 = a2
            };
        }

        public static Arguments New(JSValue @this, JSValue a0, JSValue a1, JSValue a2, JSValue a3)
        {
            return new Arguments
            {
                This = @this,
                length = 4,
                Arg0 = a0,
                Arg1 = a1,
                Arg2 = a2,
                Arg3 = a3
            };
        }

        public Arguments OverrideThis(JSValue @this)
        {
            return new Arguments
            {
                This = @this,
                length = length,
                Arg0 = Arg0,
                Arg1 = Arg1,
                Arg2 = Arg2,
                Arg3 = Arg3,
                Args = Args
            };
        }

        public static Arguments New(JSValue @this, JSValue[] args)
        {
            if (args.Length < MinArray)
            {
                throw new InvalidOperationException();
            }
            return new Arguments { 
                This = @this,
                length = args.Length,
                Args = args
            };
        }

        public JSValue Get1()
        {
            if (length == 0)
                return JSUndefined.Value;
            if (length < MinArray)
                return Arg0;
            return Args[0];
        }

        public (JSValue, JSValue) Get2()
        {
            if (length == 0)
                return (JSUndefined.Value, JSUndefined.Value);
            if (length == 1)
                return (Arg0, JSUndefined.Value);
            if (length < MinArray)
                return (Arg0, Arg1);
            return (Args[0], Args[1]);
        }

        public (JSValue, JSValue, JSValue) Get3()
        {
            if (length == 0)
                return (JSUndefined.Value, JSUndefined.Value, JSUndefined.Value);
            if (length == 1)
                return (Arg0, JSUndefined.Value, JSUndefined.Value);
            if (length == 2)
                return (Arg0, Arg1, JSUndefined.Value);
            if (length < MinArray)
                return (Arg0, Arg1, Arg2);
            return (Args[0], Args[1], Args[2]);
        }

        public (JSValue, JSValue, JSValue, JSValue) Get4()
        {
            if (length == 0)
                return (JSUndefined.Value, JSUndefined.Value, JSUndefined.Value, JSUndefined.Value);
            if (length == 1)
                return (Arg0, JSUndefined.Value, JSUndefined.Value, JSUndefined.Value);
            if (length == 2)
                return (Arg0, Arg1, JSUndefined.Value, JSUndefined.Value);
            if (length == 3)
                return (Arg0, Arg1, Arg2, JSUndefined.Value);
            if (length < MinArray)
                return (Arg0, Arg1, Arg2, Arg3);
            return (Args[0], Args[1], Args[2], Args[3]);
        }

        public JSValue[] GetArgs()
        {
            return Args;
        }
    }
}
