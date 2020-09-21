using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core.Set
{
    [JSRuntime(typeof(JSMapStatic), typeof(JSMap.JSMapPrototype))]
    public class JSSet: JSMap {

        public JSSet(): base(JSContext.Current.SetPrototype)
        {

        }

        [Constructor]
        public static JSValue Constructor(JSValue t, JSValue[] a)
        {
            return new JSSet();
        }

        [Prototype("add")]
        public static JSValue Add(JSValue t, JSValue[] a)
        {
            var f = a.Get1();
            return JSMapPrototype.Set(t, new JSValue[] { f, f });
        }

        [Prototype("set")]
        public static JSValue Set(JSValue t, JSValue[] a)
        {
            throw new NotImplementedException();
        }
    }
}
