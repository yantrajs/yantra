using System;
using YantraJS.Core.CodeGen;

namespace YantraJS.Core.Generator
{
    public delegate JSValue JSAsyncDelegate(ScriptInfo scriptInfo, JSVariable[] closures, in JSWeakAwaiter generator, in Arguments a);

    public class JSAsyncFunction : JSFunction
    {
        private readonly ScriptInfo scriptInfo;
        private readonly JSVariable[] closures;
        readonly JSAsyncDelegate @delegate;

        public JSAsyncFunction(ScriptInfo scriptInfo, JSVariable[] closures, JSAsyncDelegate @delegate, in StringSpan name, in StringSpan code) :
            base(null, name, code)
        {
            this.scriptInfo = scriptInfo;
            this.closures = closures;
            this.@delegate = @delegate;
            this.f = InvokeFunction;
        }


        public override JSValue InvokeFunction(in Arguments a)
        {
            return new JSAwaiter(scriptInfo, closures, @delegate, a);
        }

    }
}
