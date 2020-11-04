using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Runtime;
using YantraJS.Extensions;

namespace YantraJS.Core.Set
{
    [JSRuntime(typeof(JSMapStatic), typeof(JSMap.JSMapPrototype))]
    public class JSSet: JSMap {

        public JSSet(): base(JSContext.Current.SetPrototype)
        {

        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            return new JSSet();
        }

        [Prototype("add")]
        public static JSValue Add(in Arguments a)
        {
            var f = a.Get1();
            return JSMapPrototype.Set(new Arguments(a.This, f, f));
        }

        [Prototype("set")]
        public static JSValue Set(in Arguments a)
        {
            throw new NotImplementedException();
        }
    }
}
