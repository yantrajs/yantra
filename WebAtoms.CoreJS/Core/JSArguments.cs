using System;
using System.Collections.Generic;
using System.Linq;
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

        public static JSArguments From(params double[] args)
        {
            return new JSArguments(args.Select((n) => new JSNumber(n)).ToArray());
        }

        public static JSArguments From(params string[] args)
        {
            return new JSArguments(args.Select((n) => new JSString(n)).ToArray());
        }

        public JSArguments(params JSValue[] args)
        {
            _length = (uint)args.Count();
            uint i = 0;
            foreach(var item in args)
            {
                elements[i++] = item;
            }
        }

    }
}
