#nullable enable
using System.Collections.Generic;
using System.Linq;
using WebAtoms;

namespace YantraJS.Core
{
    public readonly partial struct Arguments
    {

        public IList<IJSValue> ToList()
        {
            switch (Length)
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

        public Arguments(IJSValue[] args)
        {
            NewTarget = null;
            This = JSUndefined.Value;
            Length = args.Length;
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
                    Arg0 = args[0].ToJSValue();
                    Arg1 = null;
                    Arg2 = null;
                    Arg3 = null;
                    Args = null;
                    break;
                case 2:
                    Arg0 = args[0].ToJSValue();
                    Arg1 = args[1].ToJSValue();
                    Arg2 = null;
                    Arg3 = null;
                    Args = null;
                    break;
                case 3:
                    Arg0 = args[0].ToJSValue();
                    Arg1 = args[1].ToJSValue();
                    Arg2 = args[2].ToJSValue();
                    Arg3 = null;
                    Args = null;
                    break;
                case 4:
                    Arg0 = args[0].ToJSValue();
                    Arg1 = args[1].ToJSValue();
                    Arg2 = args[2].ToJSValue();
                    Arg3 = args[3].ToJSValue();
                    Args = null;
                    break;
                default:
                    Arg0 = null;
                    Arg1 = null;
                    Arg2 = null;
                    Arg3 = null;
                    Args = args.Select(x => x.ToJSValue()).ToArray();
                    break;
            }

        }

        public Arguments(IJSValue thisArg, IJSValue[] args)
        {
            NewTarget = null;
            This = thisArg.ToJSValue();
            Length = args.Length;
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
                    Arg0 = args[0].ToJSValue();
                    Arg1 = null;
                    Arg2 = null;
                    Arg3 = null;
                    Args = null;
                    break;
                case 2:
                    Arg0 = args[0].ToJSValue();
                    Arg1 = args[1].ToJSValue();
                    Arg2 = null;
                    Arg3 = null;
                    Args = null;
                    break;
                case 3:
                    Arg0 = args[0].ToJSValue();
                    Arg1 = args[1].ToJSValue();
                    Arg2 = args[2].ToJSValue();
                    Arg3 = null;
                    Args = null;
                    break;
                case 4:
                    Arg0 = args[0].ToJSValue();
                    Arg1 = args[1].ToJSValue();
                    Arg2 = args[2].ToJSValue();
                    Arg3 = args[3].ToJSValue();
                    Args = null;
                    break;
                default:
                    Arg0 = null;
                    Arg1 = null;
                    Arg2 = null;
                    Arg3 = null;
                    Args = args.Select(x => x.ToJSValue()).ToArray();
                    break;
            }

        }


    }
}
