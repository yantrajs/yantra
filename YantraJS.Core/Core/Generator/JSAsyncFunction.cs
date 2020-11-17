using System;

namespace YantraJS.Core.Generator
{
    public delegate JSValue JSAsyncDelegate(in JSWeakAwaiter generator, in Arguments a);

    public class JSAsyncFunction : JSFunction
    {
        readonly JSAsyncDelegate @delegate;

        public JSAsyncFunction(JSAsyncDelegate @delegate, in StringSpan name, in StringSpan code) :
            base(null, name, code)
        {
            this.@delegate = @delegate;
            this.f = InvokeFunction;
        }


        public override JSValue InvokeFunction(in Arguments a)
        {
            return new JSAwaiter(@delegate, a);
        }

    }
}
