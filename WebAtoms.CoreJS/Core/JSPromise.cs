using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core.Runtime;

namespace WebAtoms.CoreJS.Core
{

    public delegate void JSPromiseDelegate(JSValue resolve, JSValue reject);

    [JSRuntime(typeof(JSPromiseStatic), typeof(JSPromisePrototype))]
    public class JSPromise: JSObject
    {

        internal Task<JSValue> task;

        public JSPromise(JSPromiseDelegate @delegate): 
            base(JSContext.Current.PromisePrototype)
        {
            var ts = new TaskCompletionSource<JSValue>();
            
            task = ts.Task;
        }
    }
}
