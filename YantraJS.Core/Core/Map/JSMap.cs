using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using YantraJS.Core.Runtime;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    [JSRuntime(typeof(JSMapStatic), typeof(JSMap.JSMapPrototype))]
    public partial class JSMap: JSObject
    {

        private LinkedList<(JSValue key,JSValue value)> entries = new LinkedList<(JSValue,JSValue)>();
        private StringTrie<LinkedListNode<(JSValue key,JSValue value)>> cache = new StringTrie<LinkedListNode<(JSValue, JSValue)>>();

        public JSMap(): base(JSContext.Current.MapPrototype)
        {
        }

        protected JSMap(JSObject p): base(p) { }

    }
}
