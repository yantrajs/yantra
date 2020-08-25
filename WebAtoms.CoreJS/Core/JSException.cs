using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSException: Exception
    {

        public JSValue Error { get; }

        public JSException(string message): base(message)
        {
            Error = new JSError(new JSString(message), JSUndefined.Value);
        }

        public JSException(string message, JSValue prototype) : base(message)
        {
            Error = new JSError(new JSString(message), JSUndefined.Value);
            Error.prototypeChain = prototype;
        }

        public JSException(JSValue message) : base(message.ToString())
        {
            Error = new JSError(message, JSUndefined.Value);
        }

        internal static void Throw(JSValue value)
        {
            throw new JSException(value);
        }



    }
}
