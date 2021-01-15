using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Extensions;

namespace YantraJS.Core.Objects
{
    public class JSReflect: JSObject
    {

        [Static("apply", Length = 1)]
        public static JSValue Apply(in Arguments a)
        {
            var (target, thisArgument, arguments) = a.Get3();
            var fx = target as JSFunction;
            return fx.InvokeFunction(Arguments.ForApply(thisArgument, arguments));
        }

        [Static("construct", Length = 1)]
        public static JSValue Construct(in Arguments a)
        {
            var (target, arguments, newTarget) = a.Get3();
            newTarget = newTarget.IsUndefined ? target : newTarget;
            var fx = target as JSFunction;
            return fx.CreateInstance(Arguments.ForApply(new JSObject(), arguments));
        }

    }

}
