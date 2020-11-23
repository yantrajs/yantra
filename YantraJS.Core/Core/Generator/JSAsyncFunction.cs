using System;

namespace YantraJS.Core.Generator
{
    public delegate JSValue JSAsyncDelegate(JSVariable[] closures, in JSWeakAwaiter generator, in Arguments a);

    public class JSAsyncFunction : JSFunction
    {
        private readonly JSVariable[] closures;
        readonly JSAsyncDelegate @delegate;

        public JSAsyncFunction(JSVariable[] closures, JSAsyncDelegate @delegate, in StringSpan name, in StringSpan code) :
            base(null, name, code)
        {
            this.closures = closures;
            this.@delegate = @delegate;
            this.f = InvokeFunction;
        }


        public override JSValue InvokeFunction(in Arguments a)
        {
            return new JSAwaiter(closures, @delegate, a);
        }

    }
}
