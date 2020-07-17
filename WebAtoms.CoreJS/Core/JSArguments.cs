using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSArguments: JSArray
    {

        public static JSArguments Empty = new JSArguments();

        public static JSArguments From(params JSValue[] args)
        {
            return new JSArguments(args);
        }

        public JSArguments(params JSValue[] args)
        {
            _length = (uint)args.Length;
            for (uint i = 0; i < args.Length; i++)
            {
                elements[i] = args[i];
            }
        }

    }
}
