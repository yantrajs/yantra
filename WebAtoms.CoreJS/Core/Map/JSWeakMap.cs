using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    [JSRuntime(typeof(JSMapStatic), typeof(JSWeakMap.JSMapPrototype))]
    public partial class JSWeakMap: JSObject
    {

        private LinkedList<(JSValue key,WeakReference<JSValue> value)> entries = 
            new LinkedList<(JSValue, WeakReference<JSValue>)>();
        private BinaryCharMap<LinkedListNode<(JSValue key, WeakReference<JSValue> value)>> cache 
            = new BinaryCharMap<LinkedListNode<(JSValue, WeakReference<JSValue>)>>();

        public JSWeakMap(): base(JSContext.Current.WeakMapPrototype)
        {
        }

    }
}
