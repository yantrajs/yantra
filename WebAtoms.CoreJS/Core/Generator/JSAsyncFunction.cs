using System;

namespace WebAtoms.CoreJS.Core.Generator
{
    public delegate JSValue JSAsyncDelegate(in JSWeakAwaiter generator, in Arguments a);

    public class JSAsyncFunction : JSFunction
    {
        readonly JSAsyncDelegate @delegate;

        public JSAsyncFunction(JSAsyncDelegate @delegate, string name, string code) :
            base(JSFunction.empty, name, code)
        {
            this.@delegate = @delegate;
        }


        public override JSValue InvokeFunction(in Arguments a)
        {
            return new JSAwaiter(@delegate, a);
        }

    }
}
