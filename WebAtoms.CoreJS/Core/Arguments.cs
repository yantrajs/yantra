using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public struct Arguments
    {

        // public JSContext Context;

        private const int MinArray = 5;

        private int length;

        public JSValue This;

        private JSValue Arg0;

        private JSValue Arg1;

        private JSValue Arg2;

        private JSValue Arg3;

        private JSValue[] Args;


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
